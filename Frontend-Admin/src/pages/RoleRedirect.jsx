import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function RoleRedirect() {
  const { user } = useAuth();

  // ยังไม่ login
  if (!user) return <Navigate to="/login" replace />;

  // แยกตาม role
  if (user.role === "admin") return <Navigate to="/dashboard" replace />;
  if (user.role === "staff") return <Navigate to="/orders" replace />;

  // fallback (เผื่อ role อื่น)
  return <Navigate to="/login" replace />;
}
