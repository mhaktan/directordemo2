// Auto-generated flow: PurchaseRequest Approval Flow
// Auto-generated approval flow for PurchaseRequest. Customize email templates and add conditions as needed.
// Resource: PurchaseRequest
// Enabled: true
//
// Nodes:
  // trigger: On PurchaseRequest Submit
  // condition: Status = PendingManagerApproval?
  // approval: PurchaseRequest Approval
  // action: Send Approval Email
  // trigger: On PurchaseRequest Approved
  // action: Send Completion Email
//
// Edges:
  // On PurchaseRequest Submit → Status = PendingManagerApproval?
  // Status = PendingManagerApproval? → PurchaseRequest Approval (true)
  // PurchaseRequest Approval → Send Approval Email
  // On PurchaseRequest Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_PURCHASEREQUEST_APPROVAL_FLOW_ID = 'flow-PurchaseRequest-approval';
