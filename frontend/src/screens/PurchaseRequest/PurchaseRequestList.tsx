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
import { PurchaseRequestCreate } from './PurchaseRequestCreate';
import { PurchaseRequestEdit } from './PurchaseRequestEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type PurchaseRequestRecord = {
  id: string | number;
  requestNumber: string;
  justification: string;
  totalAmount?: number;
  neededDate: string;
  approvedDate?: string;
  rejectionReason?: string;
  status: string;
  employeeId: string;
  departmentId: string;
  expenseCategoryId: string;
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
  { field: 'requestNumber', header: "Request Number", sortable: true, searchable: true, filterType: 'text' },
  { field: 'justification', header: "Justification", sortable: true, searchable: true, filterType: 'text' },
  { field: 'totalAmount', header: "Total Amount", sortable: true },
  { field: 'neededDate', header: "Needed Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.neededDate ? new Date(String(row.neededDate)).toLocaleDateString() : '—' },
  { field: 'approvedDate', header: "Approved Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.approvedDate ? new Date(String(row.approvedDate)).toLocaleDateString() : '—' },
  { field: 'rejectionReason', header: "Rejection Reason", sortable: true, searchable: true, filterType: 'text' },
  { field: 'status', header: "Status", sortable: true, filterType: 'radio', filterOptions: [{ label: "Draft", value: "0" }, { label: "PendingManagerApproval", value: "1" }, { label: "PendingFinanceApproval", value: "2" }, { label: "Approved", value: "3" }, { label: "Ordered", value: "4" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Draft", ["1"]: "PendingManagerApproval", ["2"]: "PendingFinanceApproval", ["3"]: "Approved", ["4"]: "Ordered"}; return m[String(row.status ?? '')] ?? String(row.status ?? '\u2014'); } },
  { field: 'employeeId', header: "Personel", sortable: true },
  { field: 'departmentId', header: "Birim", sortable: true },
  { field: 'expenseCategoryId', header: "Harcama Kalemi", sortable: true },
];

// ---------------------------------------------------------------------------
// PurchaseRequestList
// ---------------------------------------------------------------------------

export const PurchaseRequestList: React.FC = () => {
  const list = useListQuery<PurchaseRequestRecord>({ resource: 'PurchaseRequest' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<PurchaseRequestRecord | null>(null);
  const [selectedRows, setSelectedRows] = useState<PurchaseRequestRecord[]>([]);

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
    Promise.resolve(dataProvider.getOne('PurchaseRequest', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as PurchaseRequestRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('PurchaseRequest', (ids) => { ids.forEach(id => triggerFlows('delete', 'PurchaseRequest', { id })); });


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
        title="PurchaseRequest"
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
        onSelectionChange={(rows) => setSelectedRows(rows as PurchaseRequestRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <TkButton label={`Delete (${selectedRows.length})`} variant="danger" onTkClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} record(s)`)} />
          )}

          <TkButton label="+ Create PurchaseRequest" variant="primary" onTkClick={() => setShowCreate(true)} />
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
      <PurchaseRequestCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <PurchaseRequestEdit record={editRecord} onClose={() => setEditRecord(null)} onSuccess={list.invalidate} />

    </>
  );
};

