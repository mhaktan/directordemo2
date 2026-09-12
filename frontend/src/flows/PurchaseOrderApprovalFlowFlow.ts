// Auto-generated flow: PurchaseOrder Approval Flow
// Auto-generated approval flow for PurchaseOrder. Customize email templates and add conditions as needed.
// Resource: PurchaseOrder
// Enabled: true
//
// Nodes:
  // trigger: On PurchaseOrder Submit
  // condition: Status = PendingApproval?
  // approval: PurchaseOrder Approval
  // action: Send Approval Email
  // trigger: On PurchaseOrder Approved
  // action: Send Completion Email
//
// Edges:
  // On PurchaseOrder Submit → Status = PendingApproval?
  // Status = PendingApproval? → PurchaseOrder Approval (true)
  // PurchaseOrder Approval → Send Approval Email
  // On PurchaseOrder Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_PURCHASEORDER_APPROVAL_FLOW_ID = 'flow-PurchaseOrder-approval';
