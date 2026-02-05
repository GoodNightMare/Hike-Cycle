import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Sidebar() {
  const { user } = useAuth();
  return (
    <aside className="lg:w-72 md:w-48 bg-gray-900 text-white p-4">
      <h1 className="text-xl font-bold mb-6">Hike-Cycle System</h1>

      <nav className="flex flex-col gap-3">
        {user && user.role === "admin" && (
          <>
            <Link to="/dashboard">Dashboard</Link>
            <Link to="/orders">Orders</Link>
            <Link to="/products">Products</Link>
            <Link to="/users">Users</Link>
            <Link to="/settings">Settings</Link>
          </>
        )}
        {user && user.role === "staff" && (
          <>
            <Link to="/orders">Orders</Link>
          </>
        )}
      </nav>
    </aside>
  );
}
