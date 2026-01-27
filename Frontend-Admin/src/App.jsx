import { Routes, Route } from "react-router-dom";
import AdminLayout from "./layouts/AdminLayout";
import Dashboard from "./pages/DashBoard";
import Login from "./pages/Login";
import ProtectedRoute from "./components/ProtectedRoute";
import OrderPage from "./pages/Order";
import RoleRedirect from "./pages/RoleRedirect";

export default function App() {
  return (
    <Routes>
      {/* 👇 ROOT */}
      <Route path="/" element={<RoleRedirect />} />

      <Route path="/login" element={<Login />} />

      <Route
        path="/dashboard"
        element={
          <ProtectedRoute role="admin">
            <AdminLayout>
              <Dashboard />
            </AdminLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/orders"
        element={
          <ProtectedRoute role="staff">
            <AdminLayout>
              <OrderPage />
            </AdminLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/products"
        element={
          <ProtectedRoute role="admin">
            <AdminLayout>
              <h1>Products Page</h1>
            </AdminLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/customers"
        element={
          <ProtectedRoute role="admin">
            <AdminLayout>
              <h1>Customers Page</h1>
            </AdminLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/reports"
        element={
          <ProtectedRoute role="admin">
            <AdminLayout>
              <h1>Reports Page</h1>
            </AdminLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/settings"
        element={
          <ProtectedRoute role="admin">
            <AdminLayout>
              <h1>Settings Page</h1>
            </AdminLayout>
          </ProtectedRoute>
        }
      />
    </Routes>
  );
}
