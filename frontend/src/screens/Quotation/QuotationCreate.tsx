import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkDatepicker, TkInput, TkSelect } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type QuotationRecord = {
  id: string | number;
  quotationDate: string;
  validUntil?: string;
  amount: number;
  isSelected: boolean;
  purchaseRequestId: string;
  supplierId: string;
};

interface QuotationCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const QuotationCreate: React.FC<QuotationCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<QuotationRecord>>({});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<QuotationRecord>) => dataProvider.create('Quotation', values),
    onSuccess: (_data, values) => { triggerFlows('create', 'Quotation', values as Record<string, unknown>); onSuccess(); onClose(); setForm({}); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Create Quotation</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div>
                  <TkDatepicker label="Quotation Date *" value={String(form.quotationDate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('quotationDate', v))(e.detail)} />
                </div>
                <div>
                  <TkDatepicker label="Valid Until" value={String(form.validUntil ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('validUntil', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="number" label="Amount *" value={String(form.amount ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('amount', Number(v)))(e.detail)} />
                </div>
                <div>
                  <label style={{ display: 'flex', alignItems: 'center', gap: 8, fontSize: 14 }}>
                    <input type="checkbox" checked={!!form.isSelected} onChange={(e) => ((v) => setField('isSelected', v))(e.target.checked)} style={{ width: 16, height: 16 }} />
                    Is Selected *
                  </label>
                </div>
                <div>
                  <LookupSelect label="Satın Alma Talebi *" resource="PurchaseRequest" value={String(form.purchaseRequestId ?? '')} onChange={(v) => setField('purchaseRequestId', v)} displayField="requestNumber" />
                </div>
                <div>
                  <LookupSelect label="Tedarikçi *" resource="Supplier" value={String(form.supplierId ?? '')} onChange={(v) => setField('supplierId', v)} displayField="name" />
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
