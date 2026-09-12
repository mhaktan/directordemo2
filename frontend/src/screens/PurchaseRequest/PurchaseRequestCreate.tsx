import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkDatepicker, TkInput, TkSelect } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

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
};

interface PurchaseRequestCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const PurchaseRequestCreate: React.FC<PurchaseRequestCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<PurchaseRequestRecord>>({});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<PurchaseRequestRecord>) => dataProvider.create('PurchaseRequest', values),
    onSuccess: (_data, values) => { triggerFlows('create', 'PurchaseRequest', values as Record<string, unknown>); onSuccess(); onClose(); setForm({}); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Create PurchaseRequest</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div>
                  <TkInput mode="text" label="Request Number *" value={String(form.requestNumber ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('requestNumber', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Justification *" value={String(form.justification ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('justification', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="number" label="Total Amount" value={String(form.totalAmount ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('totalAmount', Number(v)))(e.detail)} />
                </div>
                <div>
                  <TkDatepicker label="Needed Date *" value={String(form.neededDate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('neededDate', v))(e.detail)} />
                </div>
                <div>
                  <TkDatepicker label="Approved Date" value={String(form.approvedDate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('approvedDate', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Rejection Reason" value={String(form.rejectionReason ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('rejectionReason', v))(e.detail)} />
                </div>
                <div>
                  <LookupSelect label="Status *" value={String(form.status ?? '')} onChange={(v) => setField('status', v ? Number(v) : null)} searchable={false} options={[{ label: 'Draft', value: '0' }, { label: 'PendingManagerApproval', value: '1' }, { label: 'PendingFinanceApproval', value: '2' }, { label: 'Approved', value: '3' }, { label: 'Ordered', value: '4' }]} />
                </div>
                <div>
                  <LookupSelect label="Personel *" resource="Employee" value={String(form.employeeId ?? '')} onChange={(v) => setField('employeeId', v)} displayField="fullName" />
                </div>
                <div>
                  <LookupSelect label="Birim *" resource="Department" value={String(form.departmentId ?? '')} onChange={(v) => setField('departmentId', v)} displayField="name" />
                </div>
                <div>
                  <LookupSelect label="Harcama Kalemi *" resource="ExpenseCategory" value={String(form.expenseCategoryId ?? '')} onChange={(v) => setField('expenseCategoryId', v)} displayField="name" />
                </div>
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <TkButton label="Cancel" variant="secondary" onTkClick={onClose} />
            <TkButton label={mutation.isPending ? 'Saving…' : 'Create'} variant="primary" mode="submit" disabled={mutation.isPending} />
          </div>
        </form>
      </div>
    </div>
  );
};
