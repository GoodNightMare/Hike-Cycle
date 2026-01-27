import { LogOut, UserRound } from "lucide-react";
import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";

export default function Topbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <header className="w-full h-16 bg-white border-b flex items-center justify-between px-6 shadow-sm">
      {/* ซ้าย */}
      <div></div>

      {/* ขวา */}
      <div className="flex items-center gap-4">
        {/* User info */}
        <div className="flex items-center gap-2 text-sm text-gray-700">
          <UserRound size={18} />
          <span>{user?.email}</span>
          <span className="text-xs bg-gray-200 px-2 py-0.5 rounded">
            {user?.role}
          </span>
        </div>

        {/* Logout */}
        <button
          onClick={handleLogout}
          className="flex items-center gap-1 text-red-500 hover:text-red-700 text-sm transition"
        >
          <LogOut size={18} />
          ออกจากระบบ
        </button>
      </div>
    </header>
  );
}
