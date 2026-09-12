import React, { useState } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom';
import { isAuthenticated, clearAuth } from './dataProvider';
import { LoginPage } from './screens/LoginPage';
import { GlobalMenu } from './components/GlobalMenu';
import { FlowProvider } from './flows/FlowProvider';
import { DepartmentList } from './screens/Department/DepartmentList';
import { EmployeeList } from './screens/Employee/EmployeeList';
import { SupplierList } from './screens/Supplier/SupplierList';
import { ExpenseCategoryList } from './screens/ExpenseCategory/ExpenseCategoryList';
import { PurchaseRequestList } from './screens/PurchaseRequest/PurchaseRequestList';
import { PurchaseRequestItemList } from './screens/PurchaseRequestItem/PurchaseRequestItemList';
import { QuotationList } from './screens/Quotation/QuotationList';
import { PurchaseOrderList } from './screens/PurchaseOrder/PurchaseOrderList';
import { DashboardScreen } from './screens/dashboard/DashboardScreen';
import TaskInboxScreen from './screens/tasks/TaskInboxScreen';
import UserListScreen from './admin/UserListScreen';
import RoleListScreen from './admin/RoleListScreen';

const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 1, staleTime: 0 } },
});

export const App: React.FC = () => {
  const [authenticated, setAuthenticated] = useState(isAuthenticated());


  const handleLogout = () => {
    clearAuth();
    setAuthenticated(false);
    queryClient.clear();
  };

  return (
    <QueryClientProvider client={queryClient}>
      <FlowProvider>
      <BrowserRouter>
        <Routes>
          <Route path="*" element={
            authenticated
              ? (
                <GlobalMenu>
                  <Routes>
                    <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<DashboardScreen />} />
          <Route path="/Department" element={<DepartmentList />} />
          <Route path="/Employee" element={<EmployeeList />} />
          <Route path="/Supplier" element={<SupplierList />} />
          <Route path="/ExpenseCategory" element={<ExpenseCategoryList />} />
          <Route path="/PurchaseRequest" element={<PurchaseRequestList />} />
          <Route path="/PurchaseRequestItem" element={<PurchaseRequestItemList />} />
          <Route path="/Quotation" element={<QuotationList />} />
          <Route path="/PurchaseOrder" element={<PurchaseOrderList />} />
          <Route path="/tasks" element={<TaskInboxScreen />} />
          <Route path="/users" element={<UserListScreen />} />
          <Route path="/roles" element={<RoleListScreen />} />
                  </Routes>
                </GlobalMenu>
              )
              : <LoginPage onLogin={() => setAuthenticated(true)} />
          } />
        </Routes>
      </BrowserRouter>
        </FlowProvider>
    </QueryClientProvider>
  );
};
