import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkInput, TkSelect } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type PurchaseRequestItemRecord = {
  id: string | number;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal?: number;
  purchaseRequestId: string;
};

interface PurchaseRequestItemCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const PurchaseRequestItemCreate: React.FC<PurchaseRequestItemCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<PurchaseRequestItemRecord>>({});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<PurchaseRequestItemRecord>) => dataProvider.create('PurchaseRequestItem', values),
    onSuccess: (_data, values) => { triggerFlows('create', 'PurchaseRequestItem', values as Record<string, unknown>); onSuccess(); onClose(); setForm({}); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Create PurchaseRequestItem</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div>
                  <TkInput mode="text" label="Product Name *" value={String(form.productName ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('productName', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="number" label="Quantity *" value={String(form.quantity ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('quantity', Number(v)))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="number" label="Unit Price *" value={String(form.unitPrice ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('unitPrice', Number(v)))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="number" label="Line Total" value={String(form.lineTotal ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('lineTotal', Number(v)))(e.detail)} />
                </div>
                <div>
                  <LookupSelect label="Satın Alma Talebi *" resource="PurchaseRequest" value={String(form.purchaseRequestId ?? '')} onChange={(v) => setField('purchaseRequestId', v)} displayField="requestNumber" />
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
