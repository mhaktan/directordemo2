import React from 'react';
import { UiCard } from '../../shared/ui';

import {
  ResponsiveContainer,
  LineChart, Line,
  BarChart, Bar,
  PieChart, Pie, Cell,
  CartesianGrid, XAxis, YAxis, Tooltip,
} from 'recharts';

import { API_BASE } from '../../config';
import { getRequestHeaders } from '../../dataProvider';


const LOADING_KEYFRAMES = `
@keyframes pulse { 0%, 100% { opacity: 1; } 50% { opacity: 0.4; } }
@keyframes spin { to { transform: rotate(360deg); } }
`;

export const DashboardScreen: React.FC = () => {
  const getNestedValue = (obj: Record<string, unknown>, path: string): unknown => {
    const direct = path.split('.').reduce<unknown>((o, k) => (o && typeof o === 'object') ? (o as Record<string, unknown>)[k] : undefined, obj);
    if (direct !== undefined) return direct;
    // Fallback: try last segment at top level (handles ABP-style {result: {...}} unwrapping)
    const segments = path.split('.');
    if (segments.length > 1 && obj && typeof obj === "object") {
      const lastKey = segments[segments.length - 1];
      const top = (obj as Record<string, unknown>)[lastKey];
      if (top !== undefined) return top;
    }
    return undefined;
  };

  // Unwrap common API envelopes: ABP {result, __abp}, generic {data}, etc.
  const unwrapResponse = (json: unknown): unknown => {
    if (!json || typeof json !== "object" || Array.isArray(json)) return json;
    const obj = json as Record<string, unknown>;
    // ABP envelope: {result, success, error, __abp}
    if ("__abp" in obj && "result" in obj) return obj.result;
    // Generic envelope: {success: true, data: X}
    if ("success" in obj && "data" in obj && Object.keys(obj).length <= 4) return obj.data;
    return json;
  };

  const extractArray = (json: unknown): Record<string, unknown>[] => {
    if (Array.isArray(json)) return json;
    if (json && typeof json === 'object') {
      const obj = json as Record<string, unknown>;
      for (const key of ['items', 'data', 'results', 'records', 'rows', 'list']) {
        if (Array.isArray(obj[key])) return obj[key] as Record<string, unknown>[];
      }
      // Recurse one level — handles {result: {items: [...]}}
      for (const val of Object.values(obj)) {
        if (val && typeof val === 'object' && !Array.isArray(val)) {
          const inner = val as Record<string, unknown>;
          for (const key of ['items', 'data', 'results', 'records', 'rows', 'list']) {
            if (Array.isArray(inner[key])) return inner[key] as Record<string, unknown>[];
          }
        }
        if (Array.isArray(val)) return val as Record<string, unknown>[];
      }
    }
    return [];
  };

  const [mytasks_0Data, setMytasks_0Data] = React.useState<Record<string, unknown> | null>(null);
  const [mytasks_0Loading, setMytasks_0Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/Approval/GetMyPendingApprovals`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        setMytasks_0Data(json);
      } catch { /* ignore */ }
      finally { setMytasks_0Loading(false); }
    };
    fetchData();
  }, []);

  const [count_purchaseRequest_1Data, setCount_purchaseRequest_1Data] = React.useState<Record<string, unknown> | null>(null);
  const [count_purchaseRequest_1Loading, setCount_purchaseRequest_1Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/PurchaseRequest/GetAll?StatusIn=0%2C1%2C2%2C3&MaxResultCount=1`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        setCount_purchaseRequest_1Data(json);
      } catch { /* ignore */ }
      finally { setCount_purchaseRequest_1Loading(false); }
    };
    fetchData();
  }, []);

  const [count_purchaseOrder_2Data, setCount_purchaseOrder_2Data] = React.useState<Record<string, unknown> | null>(null);
  const [count_purchaseOrder_2Loading, setCount_purchaseOrder_2Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/PurchaseOrder/GetAll?StatusIn=2%2C3&MaxResultCount=1`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        setCount_purchaseOrder_2Data(json);
      } catch { /* ignore */ }
      finally { setCount_purchaseOrder_2Loading(false); }
    };
    fetchData();
  }, []);

  const [breakdown_purchaseRequest_3Data, setBreakdown_purchaseRequest_3Data] = React.useState<Record<string, unknown>[]>([]);
  const [breakdown_purchaseRequest_3Loading, setBreakdown_purchaseRequest_3Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/PurchaseRequest/GetGroupedCount?GroupBy=Status`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result') ?? getNestedValue(json, 'result');
        setBreakdown_purchaseRequest_3Data(extractArray(target ?? json));
      } catch { /* ignore */ }
      finally { setBreakdown_purchaseRequest_3Loading(false); }
    };
    fetchData();
  }, []);

  const [breakdown_purchaseRequest_4Data, setBreakdown_purchaseRequest_4Data] = React.useState<Record<string, unknown>[]>([]);
  const [breakdown_purchaseRequest_4Loading, setBreakdown_purchaseRequest_4Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/PurchaseRequest/GetGroupedCount?GroupBy=DepartmentId`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result') ?? getNestedValue(json, 'result');
        setBreakdown_purchaseRequest_4Data(extractArray(target ?? json));
      } catch { /* ignore */ }
      finally { setBreakdown_purchaseRequest_4Loading(false); }
    };
    fetchData();
  }, []);

  const [list_purchaseRequest_5_tableData, setList_purchaseRequest_5_tableData] = React.useState<Record<string, unknown> | null>(null);
  const [list_purchaseRequest_5_tableLoading, setList_purchaseRequest_5_tableLoading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/PurchaseRequest/GetAll?StatusIn=0%2C1%2C2&NeededDateTo=2026-09-12&MaxResultCount=10&Sorting=id%20desc`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result.items') ?? getNestedValue(json, 'result.items');
        setList_purchaseRequest_5_tableData(target != null ? (target as Record<string, unknown>) : json);
      } catch { /* ignore */ }
      finally { setList_purchaseRequest_5_tableLoading(false); }
    };
    fetchData();
  }, []);

  const [list_purchaseOrder_6_tableData, setList_purchaseOrder_6_tableData] = React.useState<Record<string, unknown> | null>(null);
  const [list_purchaseOrder_6_tableLoading, setList_purchaseOrder_6_tableLoading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/PurchaseOrder/GetAll?StatusIn=2%2C3&ExpectedDeliveryDateTo=2026-09-12&MaxResultCount=10&Sorting=id%20desc`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result.items') ?? getNestedValue(json, 'result.items');
        setList_purchaseOrder_6_tableData(target != null ? (target as Record<string, unknown>) : json);
      } catch { /* ignore */ }
      finally { setList_purchaseOrder_6_tableLoading(false); }
    };
    fetchData();
  }, []);

  return (
    <div>
      <style dangerouslySetInnerHTML={{ __html: LOADING_KEYFRAMES }} />
      <h1 style={{ margin: '0 0 24px', fontSize: 22, fontWeight: 700 }}>Dashboard</h1>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, 1fr)', gap: 16 }}>
        <div style={{ gridColumn: 'span 12' }}>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 16 }}>
            <div style={{ gridColumn: 'span 1' }}>
              {mytasks_0Loading ? (
                <UiCard bodyStyle={{ padding: 20 }}>
                  <div style={{ height: 12, width: '40%', background: '#e0e0e0', borderRadius: 4, marginBottom: 12, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 28, width: '60%', background: '#e0e0e0', borderRadius: 4, marginBottom: 8, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 10, width: '50%', background: '#f0f0f0', borderRadius: 4, animation: 'pulse 1.5s ease-in-out infinite' }} />
                </UiCard>
              ) : (
              <UiCard bodyStyle={{ padding: 20 }}>
                <div style={{ fontSize: 12, color: '#888', marginBottom: 4 }}>Onayımı Bekleyen Talepler / Siparişler</div>
                <div style={{ fontSize: 28, fontWeight: 700, color: '#1976d2' }}>{Array.isArray((Array.isArray(mytasks_0Data) ? mytasks_0Data as unknown[] : mytasks_0Data?.['result'])) ? ((Array.isArray(mytasks_0Data) ? mytasks_0Data as unknown[] : mytasks_0Data?.['result']) as unknown[]).length : (((Array.isArray(mytasks_0Data) ? mytasks_0Data as unknown[] : mytasks_0Data?.['result']) as string | number) ?? '—')}</div>
              </UiCard>
              )}

            </div>
            <div style={{ gridColumn: 'span 1' }}>
              {count_purchaseRequest_1Loading ? (
                <UiCard bodyStyle={{ padding: 20 }}>
                  <div style={{ height: 12, width: '40%', background: '#e0e0e0', borderRadius: 4, marginBottom: 12, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 28, width: '60%', background: '#e0e0e0', borderRadius: 4, marginBottom: 8, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 10, width: '50%', background: '#f0f0f0', borderRadius: 4, animation: 'pulse 1.5s ease-in-out infinite' }} />
                </UiCard>
              ) : (
              <UiCard bodyStyle={{ padding: 20 }}>
                <div style={{ fontSize: 12, color: '#888', marginBottom: 4 }}>Açık Talepler</div>
                <div style={{ fontSize: 28, fontWeight: 700, color: '#4caf50' }}>{(getNestedValue(count_purchaseRequest_1Data ?? {}, 'result.totalCount') as string | number) ?? '—'}</div>
              </UiCard>
              )}

            </div>
            <div style={{ gridColumn: 'span 1' }}>
              {count_purchaseOrder_2Loading ? (
                <UiCard bodyStyle={{ padding: 20 }}>
                  <div style={{ height: 12, width: '40%', background: '#e0e0e0', borderRadius: 4, marginBottom: 12, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 28, width: '60%', background: '#e0e0e0', borderRadius: 4, marginBottom: 8, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 10, width: '50%', background: '#f0f0f0', borderRadius: 4, animation: 'pulse 1.5s ease-in-out infinite' }} />
                </UiCard>
              ) : (
              <UiCard bodyStyle={{ padding: 20 }}>
                <div style={{ fontSize: 12, color: '#888', marginBottom: 4 }}>Teslim Beklenen Siparişler</div>
                <div style={{ fontSize: 28, fontWeight: 700, color: '#ff9800' }}>{(getNestedValue(count_purchaseOrder_2Data ?? {}, 'result.totalCount') as string | number) ?? '—'}</div>
              </UiCard>
              )}

            </div>
          </div>
        </div>
        <div style={{ gridColumn: 'span 6' }}>
          {breakdown_purchaseRequest_3Loading ? (
            <UiCard header="Durum Bazında Talep Dağılımı" bodyStyle={{ padding: 16, display: 'flex', alignItems: 'center', justifyContent: 'center', height: 280 }}>
              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                <div style={{ width: 32, height: 32, border: '3px solid #e0e0e0', borderTopColor: '#1976d2', borderRadius: '50%', animation: 'spin 0.8s linear infinite' }} />
                <span style={{ fontSize: 12, color: '#999' }}>Loading...</span>
              </div>
            </UiCard>
          ) : (
          <UiCard header="Durum Bazında Talep Dağılımı" bodyStyle={{ padding: 16 }}>
            <ResponsiveContainer width="100%" height={280}>
                  <PieChart>
                    <Pie data={breakdown_purchaseRequest_3Data} dataKey="count" nameKey="label" cx="50%" cy="50%" outerRadius={80} label>
                      <Cell fill="#1976d2" />
                      <Cell fill="#ff9800" />
                      <Cell fill="#4caf50" />
                      <Cell fill="#e91e63" />
                      <Cell fill="#9c27b0" />
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
          </UiCard>
          )}
        </div>
        <div style={{ gridColumn: 'span 6' }}>
          {breakdown_purchaseRequest_4Loading ? (
            <UiCard header="Birim Bazında Talep Tutarı" bodyStyle={{ padding: 16, display: 'flex', alignItems: 'center', justifyContent: 'center', height: 280 }}>
              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                <div style={{ width: 32, height: 32, border: '3px solid #e0e0e0', borderTopColor: '#1976d2', borderRadius: '50%', animation: 'spin 0.8s linear infinite' }} />
                <span style={{ fontSize: 12, color: '#999' }}>Loading...</span>
              </div>
            </UiCard>
          ) : (
          <UiCard header="Birim Bazında Talep Tutarı" bodyStyle={{ padding: 16 }}>
            <ResponsiveContainer width="100%" height={280}>
                  <BarChart data={breakdown_purchaseRequest_4Data}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="label" />
                    <YAxis />
                    <Tooltip />
                    <Bar dataKey="count" fill="#1976d2" />
                  </BarChart>
                </ResponsiveContainer>
          </UiCard>
          )}
        </div>
        <div style={{ gridColumn: 'span 12' }}>
          <h3 style={{ fontSize: 16, fontWeight: 600, margin: '0 0 12px' }}>İhtiyaç Tarihi Geçmiş — Onaylanmamış Talepler</h3>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(1, 1fr)', gap: 16 }}>
            <div style={{ gridColumn: 'span 1' }}>
              <div style={{ background: '#fff', borderRadius: 8, overflow: 'hidden', border: '1px solid #e8e8e8' }}>
                {list_purchaseRequest_5_tableLoading ? (
                  <div style={{ padding: 20, textAlign: 'center', color: '#999' }}>Loading...</div>
                ) : (
                  <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                    <thead><tr><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Request Number</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Needed Date</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Total Amount</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Status</th></tr></thead>
                    <tbody>
                      {(Array.isArray(list_purchaseRequest_5_tableData) ? list_purchaseRequest_5_tableData : extractArray(list_purchaseRequest_5_tableData)).map((row: Record<string, unknown>, i: number) => (
                        <tr key={i}><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['requestNumber'] ?? '')}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['neededDate'] ?? '')}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['totalAmount'] ?? '')}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['status'] ?? '')}</td></tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </div>
            </div>
          </div>
        </div>
        <div style={{ gridColumn: 'span 12' }}>
          <h3 style={{ fontSize: 16, fontWeight: 600, margin: '0 0 12px' }}>Tahmini Teslimat Tarihi Geçmiş Siparişler</h3>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(1, 1fr)', gap: 16 }}>
            <div style={{ gridColumn: 'span 1' }}>
              <div style={{ background: '#fff', borderRadius: 8, overflow: 'hidden', border: '1px solid #e8e8e8' }}>
                {list_purchaseOrder_6_tableLoading ? (
                  <div style={{ padding: 20, textAlign: 'center', color: '#999' }}>Loading...</div>
                ) : (
                  <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                    <thead><tr><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Order Number</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Order Date</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Expected Delivery Date</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Status</th></tr></thead>
                    <tbody>
                      {(Array.isArray(list_purchaseOrder_6_tableData) ? list_purchaseOrder_6_tableData : extractArray(list_purchaseOrder_6_tableData)).map((row: Record<string, unknown>, i: number) => (
                        <tr key={i}><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['orderNumber'] ?? '')}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['orderDate'] ?? '')}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['expectedDeliveryDate'] ?? '')}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['status'] ?? '')}</td></tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
