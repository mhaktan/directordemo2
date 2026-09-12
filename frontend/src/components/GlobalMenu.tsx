import React, { useState, useEffect } from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import { hasRole } from '../roles';
import { clearAuth } from '../dataProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

interface MenuItem {
  label: string;
  path: string;
  icon?: string;
  requiredRoles?: string[];
  children?: MenuItem[];
}

/** Filter menu items by user roles (recursive for groups) */
const filterByRole = (items: MenuItem[]): MenuItem[] =>
  items
    .filter((item) => hasRole(item.requiredRoles ?? []))
    .map((item) =>
      item.children ? { ...item, children: filterByRole(item.children) } : item
    )
    .filter((item) => !item.children || item.children.length > 0);

// ---------------------------------------------------------------------------
// Menu Configuration
// ---------------------------------------------------------------------------

const MENU_ITEMS: MenuItem[] = [
  {
    "label": "My Tasks",
    "path": "/tasks",
    "icon": "task_alt"
  },
  {
    "label": "Dashboard",
    "path": "/dashboard",
    "icon": "dashboard"
  },
  {
    "label": "Department",
    "path": "/Department"
  },
  {
    "label": "Employee",
    "path": "/Employee"
  },
  {
    "label": "Supplier",
    "path": "/Supplier"
  },
  {
    "label": "ExpenseCategory",
    "path": "/ExpenseCategory"
  },
  {
    "label": "PurchaseRequest",
    "path": "/PurchaseRequest"
  },
  {
    "label": "PurchaseRequestItem",
    "path": "/PurchaseRequestItem"
  },
  {
    "label": "Quotation",
    "path": "/Quotation"
  },
  {
    "label": "PurchaseOrder",
    "path": "/PurchaseOrder"
  },
  {
    "label": "Administration",
    "path": "#",
    "icon": "admin_panel_settings",
    "requiredRoles": [
      "Admin"
    ],
    "children": [
      {
        "label": "Users",
        "path": "/users",
        "icon": "people"
      },
      {
        "label": "Roles",
        "path": "/roles",
        "icon": "security"
      }
    ]
  }
];

// ---------------------------------------------------------------------------
// Icon Component (Material Symbols)
// ---------------------------------------------------------------------------

const Icon: React.FC<{ name?: string; size?: number; className?: string }> = ({ name, size = 20, className }) => {
  if (!name) return null;
  return (
    <span
      className={`material-symbols-outlined ${className ?? ''}`}
      style={{ fontSize: size, lineHeight: 1, flexShrink: 0, userSelect: 'none' }}
    >
      {name}
    </span>
  );
};

// ---------------------------------------------------------------------------
// Menu Item Components
// ---------------------------------------------------------------------------

const MenuLink: React.FC<{
  item: MenuItem;
  collapsed: boolean;
  depth?: number;
}> = ({ item, collapsed, depth = 0 }) => (
  <NavLink
    to={item.path}
    title={collapsed ? item.label : undefined}
    style={({ isActive }) => ({
      display: 'flex',
      alignItems: 'center',
      gap: collapsed ? 0 : 10,
      padding: collapsed ? '10px 0' : `9px ${depth > 0 ? 12 : 12}px 9px ${12 + depth * 16}px`,
      borderRadius: 6,
      textDecoration: 'none',
      fontSize: depth > 0 ? 13 : 14,
      marginBottom: 2,
      justifyContent: collapsed ? 'center' : 'flex-start',
      background: isActive ? 'var(--primary-50)' : 'transparent',
      color: isActive ? 'var(--primary-500)' : '#555555',
      fontWeight: isActive ? 600 : 400,
      transition: 'background 0.15s, color 0.15s',
    })}
  >
    <Icon name={item.icon ?? (depth > 0 ? 'circle' : 'article')} size={depth > 0 ? 16 : 20} />
    {!collapsed && <span style={{ overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{item.label}</span>}
  </NavLink>
);

const MenuGroup: React.FC<{
  item: MenuItem;
  collapsed: boolean;
}> = ({ item, collapsed }) => {
  const location = useLocation();
  const isChildActive = item.children?.some((c) => location.pathname.startsWith(c.path)) ?? false;
  const [open, setOpen] = useState(isChildActive);

  useEffect(() => {
    if (isChildActive && !open) setOpen(true);
  }, [isChildActive]);

  // Close popup when switching between collapsed/expanded
  useEffect(() => {
    if (collapsed) setOpen(false);
  }, [collapsed]);

  const iconRef = React.useRef<HTMLDivElement>(null);
  const popupRef = React.useRef<HTMLDivElement>(null);
  const [popupPos, setPopupPos] = React.useState({ top: 0, left: 0 });

  React.useEffect(() => {
    if (!open || !collapsed) return;
    // Position popup next to icon
    if (iconRef.current) {
      const rect = iconRef.current.getBoundingClientRect();
      setPopupPos({ top: rect.top, left: rect.right + 4 });
    }
    // Close on outside click
    const handler = (e: MouseEvent) => {
      if (popupRef.current && !popupRef.current.contains(e.target as Node) &&
          iconRef.current && !iconRef.current.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener('mousedown', handler);
    return () => document.removeEventListener('mousedown', handler);
  }, [open, collapsed]);

  if (collapsed) {
    return (
      <div title={item.label}>
        <div
          ref={iconRef}
          style={{
            display: 'flex',
            justifyContent: 'center',
            padding: '10px 0',
            cursor: 'pointer',
            color: isChildActive ? 'var(--primary-500)' : '#555555',
            borderRadius: 6,
            background: isChildActive ? 'var(--primary-50)' : 'transparent',
            marginBottom: 2,
          }}
          onClick={() => setOpen(!open)}
        >
          <Icon name={item.icon ?? 'folder'} />
        </div>
        {open && (
          <div ref={popupRef} style={{
            position: 'fixed', top: popupPos.top, left: popupPos.left,
            background: '#fff', borderRadius: 8, border: '1px solid #e0e0e0',
            boxShadow: '0 4px 16px rgba(0,0,0,0.12)', minWidth: 180,
            zIndex: 99999, overflow: 'hidden', padding: '4px 0',
          }}>
            <div style={{ padding: '6px 12px', fontSize: 11, color: '#999', fontWeight: 600, textTransform: 'uppercase' }}>
              {item.label}
            </div>
            {item.children?.map((child) => (
              <div key={child.path} onClick={() => setOpen(false)}>
                <MenuLink item={child} collapsed={false} depth={0} />
              </div>
            ))}
          </div>
        )}
      </div>
    );
  }

  return (
    <div>
      <div
        onClick={() => setOpen(!open)}
        style={{
          display: 'flex',
          alignItems: 'center',
          gap: 10,
          padding: '9px 12px',
          borderRadius: 6,
          cursor: 'pointer',
          fontSize: 14,
          marginBottom: 2,
          color: isChildActive ? 'var(--primary-500)' : '#555555',
          fontWeight: isChildActive ? 600 : 400,
          background: isChildActive ? 'var(--primary-50)' : 'transparent',
          transition: 'background 0.15s',
        }}
      >
        <Icon name={item.icon ?? 'folder'} />
        <span style={{ flex: 1, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{item.label}</span>
        <Icon name={open ? 'expand_less' : 'expand_more'} size={18} />
      </div>
      {open && (
        <div>
          {item.children?.map((child) => (
            <MenuLink key={child.path} item={child} collapsed={false} depth={1} />
          ))}
        </div>
      )}
    </div>
  );
};

// ---------------------------------------------------------------------------
// GlobalMenu Component
// ---------------------------------------------------------------------------

// ---------------------------------------------------------------------------
// User Dropdown — top-right user menu with logout
// ---------------------------------------------------------------------------

const UserDropdown: React.FC = () => {
  const [open, setOpen] = useState(false);
  const ref = React.useRef<HTMLDivElement>(null);
  const btnRef = React.useRef<HTMLButtonElement>(null);
  const [pos, setPos] = React.useState({ top: 0, right: 0 });

  React.useEffect(() => {
    if (!open) return;
    const handler = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node) &&
          btnRef.current && !btnRef.current.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener('mousedown', handler);
    return () => document.removeEventListener('mousedown', handler);
  }, [open]);

  const handleToggle = () => {
    if (!open && btnRef.current) {
      const rect = btnRef.current.getBoundingClientRect();
      setPos({ top: rect.bottom + 4, right: window.innerWidth - rect.right });
    }
    setOpen(!open);
  };

  // Try to get user info from localStorage (set after login)
  const userName = localStorage.getItem('_user_name') || 'User';
  const userEmail = localStorage.getItem('_user_email') || '';
  const initials = userName.slice(0, 2).toUpperCase();

  return (
    <div style={{ position: 'relative' }}>
      <button
        ref={btnRef}
        onClick={handleToggle}
        style={{
          display: 'flex', alignItems: 'center', gap: 8,
          padding: '4px 12px', borderRadius: 8,
          border: '1px solid #e0e0e0', background: '#fff',
          cursor: 'pointer', fontSize: 13, color: '#333',
        }}
      >
        <div style={{
          width: 28, height: 28, borderRadius: '50%',
          background: 'var(--primary-500)', color: '#fff',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 11, fontWeight: 700,
        }}>
          {initials}
        </div>
        <span style={{ maxWidth: 120, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
          {userName}
        </span>
        <Icon name={open ? 'expand_less' : 'expand_more'} size={16} />
      </button>

      {open && (
        <div ref={ref} style={{
          position: 'fixed', top: pos.top, right: pos.right,
          background: '#fff', borderRadius: 8, border: '1px solid #e0e0e0',
          boxShadow: '0 4px 16px rgba(0,0,0,0.1)', minWidth: 200,
          zIndex: 10000, overflow: 'hidden',
        }}>
          {userEmail && (
            <div style={{ padding: '12px 16px', borderBottom: '1px solid #f0f0f0', fontSize: 12, color: '#999' }}>
              {userEmail}
            </div>
          )}
          <UserMenuItem icon="person" label="Profile" onClick={() => setOpen(false)} />
          <UserMenuItem icon="settings" label="Settings" onClick={() => setOpen(false)} />
          <div style={{ height: 1, background: '#f0f0f0' }} />
          <UserMenuItem icon="logout" label="Logout" onClick={() => { clearAuth(); window.location.reload(); setOpen(false); }} danger />
        </div>
      )}
    </div>
  );
};

const UserMenuItem: React.FC<{ icon: string; label: string; onClick: () => void; danger?: boolean }> = ({ icon, label, onClick, danger }) => (
  <button
    onClick={onClick}
    style={{
      display: 'flex', alignItems: 'center', gap: 10, width: '100%',
      padding: '10px 16px', border: 'none', background: 'none',
      cursor: 'pointer', fontSize: 13, color: danger ? '#d32f2f' : '#333',
      textAlign: 'left',
    }}
    onMouseEnter={(e) => { (e.currentTarget).style.background = '#f5f5f5'; }}
    onMouseLeave={(e) => { (e.currentTarget).style.background = 'none'; }}
  >
    <Icon name={icon} size={18} />
    {label}
  </button>
);

// ---------------------------------------------------------------------------
// GlobalMenu
// ---------------------------------------------------------------------------

export const GlobalMenu: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [collapsed, setCollapsed] = useState(false);

  // Auto-collapse sidebar on small screens instead of switching to mobile drawer
  useEffect(() => {
    const onResize = () => {
      if (window.innerWidth < 768) setCollapsed(true);
    };
    onResize();
    window.addEventListener('resize', onResize);
    return () => window.removeEventListener('resize', onResize);
  }, []);

  const sidebarWidth = collapsed ? 64 : 240;

  const sidebar = (
    <aside
      style={{
        width: sidebarWidth,
        height: '100dvh',
        borderRight: '1px solid var(--tof-color-border, #e8e8e8)',
        background: '#ffffff',
        display: 'flex',
        flexDirection: 'column',
        flexShrink: 0,
        transition: 'width 0.2s ease',
        overflow: 'hidden',
      }}
    >
      {/* Header */}
      <div
        style={{
          padding: collapsed ? '16px 8px' : '16px 12px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: collapsed ? 'center' : 'space-between',
          minHeight: 56,
          flexShrink: 0,
        }}
      >
        {!collapsed && (
          <div style={{
            fontWeight: 700,
            fontSize: 17,
            color: 'var(--primary-500)',
            display: 'flex',
            alignItems: 'center',
            overflow: 'hidden',
            whiteSpace: 'nowrap',
          }}>
            <img src="data:image/png;base64,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" alt="" style={{ height: 24, marginRight: 8 }} />{!collapsed && "directordemo2"}
          </div>
        )}
        <button
          onClick={() => setCollapsed(!collapsed)}
          title={collapsed ? 'Expand menu' : 'Collapse menu'}
          style={{
            background: 'none',
            border: 'none',
            cursor: 'pointer',
            padding: 4,
            borderRadius: 4,
            color: '#555555',
            display: 'flex',
            alignItems: 'center',
          }}
        >
          <Icon name={collapsed ? 'menu_open' : 'menu'} size={22} />
        </button>
      </div>

      {/* Navigation */}
      <nav style={{ flex: 1, overflow: 'auto', padding: collapsed ? '0 8px' : '0 8px' }}>
        {filterByRole(MENU_ITEMS).map((item) =>
          item.children && item.children.length > 0 ? (
            <MenuGroup key={item.label} item={item} collapsed={collapsed} />
          ) : (
            <MenuLink key={item.path} item={item} collapsed={collapsed} />
          )
        )}
      </nav>

      {/* Footer — version info */}
      <div style={{ flexShrink: 0, padding: '8px 12px', borderTop: '1px solid #e8e8e8', fontSize: 10, color: '#bbb', textAlign: 'center' }}>
        Powered by Archipid
      </div>
    </aside>
  );

  return (
    <div style={{ display: 'flex', height: '100dvh', overflow: 'hidden', fontFamily: 'var(--tof-font-family, system-ui, sans-serif)' }}>
      {/* Sidebar — always inline, collapses to icon-only on small screens */}
      {sidebar}

      {/* Main content */}
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden', minWidth: 0, minHeight: 0 }}>
        {/* Top header bar */}
        <div style={{
          display: 'flex', alignItems: 'center', justifyContent: 'flex-end',
          padding: '8px 24px', borderBottom: '1px solid #e8e8e8', background: '#fff',
          flexShrink: 0, gap: 12,
        }}>
          <UserDropdown />
        </div>

        <main style={{ flex: 1, overflow: 'auto', padding: '32px 36px', background: 'var(--tof-color-bg-subtle, #f8f9fa)' }}>
          {children}
        </main>
      </div>
    </div>
  );
};
