// Route-level code splitting in React.
import { lazy, Suspense } from 'react';
import { BrowserRouter, Route, Routes } from 'react-router';

const OrdersPage = lazy(() => import('./pages/OrdersPage'));
const SettingsPage = lazy(() => import('./pages/SettingsPage'));

export function App() {
  return (
    <BrowserRouter>
      <Suspense fallback={<div role="status">Loading…</div>}>
        <Routes>
          <Route path="/orders" element={<OrdersPage />} />
          <Route path="/settings" element={<SettingsPage />} />
        </Routes>
      </Suspense>
    </BrowserRouter>
  );
}
