import { Routes, Route } from "react-router-dom";
import AdminLayout from "./layouts/AdminLayout";
import Dashboard from "./pages/DashBoard";
import Login from "./pages/Login";
import ProtectedRoute from "./components/ProtectedRoute";
import OrdersPage from "./pages/Orders";
import RoleRedirect from "./pages/RoleRedirect";
import UsersPage from "./pages/Users";
import ProductsPage from "./pages/Products";

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
          <ProtectedRoute roles={["admin", "staff"]}>
            <AdminLayout>
              <OrdersPage />
            </AdminLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/products"
        element={
          <ProtectedRoute role="admin">
            <AdminLayout>
              <ProductsPage />
            </AdminLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/users"
        element={
          <ProtectedRoute role="admin">
            <AdminLayout>
              <UsersPage />
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
