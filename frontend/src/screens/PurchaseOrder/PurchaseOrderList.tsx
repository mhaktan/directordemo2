import React, { useState, useCallback, useEffect, useRef } from 'react';
import { useSearchParams } from 'react-router-dom';
import { TkButton } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { useListQuery } from '../../shared/useListQuery';
import { useDeleteMutation } from '../../shared/useDeleteMutation';
import { DeleteConfirmDialog } from '../../shared/DeleteConfirmDialog';
import { ListPageLayout } from '../../shared/ListPageLayout';
import type { TableColumn } from '../../shared/ListPageLayout';
import { actionColumn } from '../../shared/ActionButtons';
import { PurchaseOrderCreate } from './PurchaseOrderCreate';
import { PurchaseOrderEdit } from './PurchaseOrderEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type PurchaseOrderRecord = {
  id: string | number;
  orderNumber: string;
  orderDate: string;
  expectedDeliveryDate?: string;
  deliveryDate?: string;
  orderAmount: number;
  notes?: string;
  status: string;
  purchaseRequestId: string;
  supplierId: string;
  [key: string]: unknown;
};

// ---------------------------------------------------------------------------
// Column definition — edit this array to add/remove/reorder columns
// ---------------------------------------------------------------------------
//
// Override examples:
//   • Hide a column:     remove its entry from COLUMNS
//   • Add a custom col:  { field: 'fullName', header: 'Full Name', html: (row) => `${row.firstName} ${row.lastName}` }
//   • Enable filtering:  add searchable: true  or  filterType: 'text' | 'checkbox' | 'radio' | 'datepicker'
//   • Custom cell render: html: (row) => `<span style="color:green">${row.status}</span>`
//
// Shared components (src/shared/) can be edited to change behavior globally:
//   • ListPageLayout  — table wrapper, pagination, header layout
//   • ActionButtons   — edit/delete button styles, labels, and behavior
//   • useListQuery    — data fetching, sorting, filtering logic
//   • useDeleteMutation / DeleteConfirmDialog — delete flow
//
// Action buttons override: edit src/shared/ActionButtons.ts DEFAULT_CONFIG
// or pass custom config: actionColumn('id', { hasEdit: true, config: { edit: { label: 'View', style: '...' } } })
//

const COLUMNS: TableColumn[] = [
  { field: 'id', header: 'ID', sortable: true },
  { field: 'orderNumber', header: "Order Number", sortable: true, searchable: true, filterType: 'text' },
  { field: 'orderDate', header: "Order Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.orderDate ? new Date(String(row.orderDate)).toLocaleDateString() : '—' },
  { field: 'expectedDeliveryDate', header: "Expected Delivery Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.expectedDeliveryDate ? new Date(String(row.expectedDeliveryDate)).toLocaleDateString() : '—' },
  { field: 'deliveryDate', header: "Delivery Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.deliveryDate ? new Date(String(row.deliveryDate)).toLocaleDateString() : '—' },
  { field: 'orderAmount', header: "Order Amount", sortable: true },
  { field: 'notes', header: "Notes", sortable: true, searchable: true, filterType: 'text' },
  { field: 'status', header: "Status", sortable: true, filterType: 'radio', filterOptions: [{ label: "Draft", value: "0" }, { label: "PendingApproval", value: "1" }, { label: "Approved", value: "2" }, { label: "Shipped", value: "3" }, { label: "Delivered", value: "4" }, { label: "Cancelled", value: "5" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Draft", ["1"]: "PendingApproval", ["2"]: "Approved", ["3"]: "Shipped", ["4"]: "Delivered", ["5"]: "Cancelled"}; return m[String(row.status ?? '')] ?? String(row.status ?? '\u2014'); } },
  { field: 'purchaseRequestId', header: "Satın Alma Talebi", sortable: true },
  { field: 'supplierId', header: "Tedarikçi", sortable: true },
];

// ---------------------------------------------------------------------------
// PurchaseOrderList
// ---------------------------------------------------------------------------

export const PurchaseOrderList: React.FC = () => {
  const list = useListQuery<PurchaseOrderRecord>({ resource: 'PurchaseOrder' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<PurchaseOrderRecord | null>(null);
  const [selectedRows, setSelectedRows] = useState<PurchaseOrderRecord[]>([]);

  // ?focus=<id> ile gelindiginde kayit dogrudan acilir (My Tasks -> kayit)
  const [searchParams, setSearchParams] = useSearchParams();
  const focusId = searchParams.get('focus');
  const focusedRef = useRef<string | null>(null);
  useEffect(() => {
    if (!focusId || focusedRef.current === focusId) return;
    focusedRef.current = focusId;
    const clear = () => {
      const next = new URLSearchParams(searchParams);
      next.delete('focus');
      setSearchParams(next, { replace: true });
    };
    Promise.resolve(dataProvider.getOne('PurchaseOrder', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as PurchaseOrderRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('PurchaseOrder', (ids) => { ids.forEach(id => triggerFlows('delete', 'PurchaseOrder', { id })); });


  const columns = [...COLUMNS, ...actionColumn('id', { hasEdit: true, hasDelete: true })];

  const handleCrudAction = useCallback((action: string, id: string) => {
    const row = list.records.find((r) => String(r.id) === id);
    if (!row) return;
    if (action === 'edit') setEditRecord(row);
    if (action === 'delete') del.requestSingleDelete(row.id);
  }, [list.records]);

  return (
    <>
      <ListPageLayout
        title="PurchaseOrder"
        subtitle={Object.keys(list.displayParams).length > 0 ? (
          <div style={{ fontSize: 13, color: '#666', marginTop: 4 }}>
            {Object.entries(list.displayParams).map(([k, v]) => (
              <span key={k} style={{ marginRight: 12 }}>{k}: <strong>{v}</strong></span>
            ))}
          </div>
        ) : undefined}
        records={list.records}
        columns={columns}
        dataKey="id"
        total={list.total}
        loading={list.isLoading}
        page={list.page}
        perPage={list.perPage}
        onPageChange={list.setPage}
        onPerPageChange={list.setPerPage}
        onTableRequest={list.handleTableRequest}
        selectionMode="checkbox"
        selectedRows={selectedRows}
        onSelectionChange={(rows) => setSelectedRows(rows as PurchaseOrderRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <TkButton label={`Delete (${selectedRows.length})`} variant="danger" onTkClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} record(s)`)} />
          )}

          <TkButton label="+ Create PurchaseOrder" variant="primary" onTkClick={() => setShowCreate(true)} />
        </>}
      />

      {list.isError && (
        <div style={{ padding: '10px 14px', background: '#fff3f3', border: '1px solid #f5c6c6', borderRadius: 6, color: '#c62828', fontSize: 13, marginBottom: 12 }}>
          Failed to load data: {(list.error as Error).message}
        </div>
      )}

      <DeleteConfirmDialog
        visible={!!del.deleteTarget}
        label={del.deleteTarget?.label ?? ''}
        isPending={del.isPending}
        onConfirm={del.confirmDelete}
        onCancel={() => del.setDeleteTarget(null)}
      />
      <PurchaseOrderCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <PurchaseOrderEdit record={editRecord} onClose={() => setEditRecord(null)} onSuccess={list.invalidate} />

    </>
  );
};

