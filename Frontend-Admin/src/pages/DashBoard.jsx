import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";

export default function Dashboard() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">
          Dashboard
        </h1>

        <button
          onClick={handleLogout}
          className="px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600"
        >
          Logout
        </button>
      </div>

      {/* User Card */}
      <div className="bg-white rounded-xl shadow p-6 mb-6">
        <h2 className="text-xl font-semibold mb-2">
          ยินดีต้อนรับ 👋
        </h2>

        <p><span className="font-medium">ชื่อ:</span> {user?.name}</p>
        <p><span className="font-medium">อีเมล:</span> {user?.email}</p>
        <p>
          <span className="font-medium">บทบาท:</span>{" "}
          <span className="capitalize text-blue-600 font-semibold">
            {user?.role}
          </span>
        </p>
      </div>

      {/* Role-based Section */}
      {user?.role === "admin" && (
        <div className="bg-white rounded-xl shadow p-6 mb-4">
          <h3 className="text-lg font-semibold mb-2">Admin Panel</h3>
          <p className="text-gray-600">
            จัดการผู้ใช้, สิทธิ์, ระบบทั้งหมด
          </p>
        </div>
      )}

      {user?.role === "staff" && (
        <div className="bg-white rounded-xl shadow p-6 mb-4">
          <h3 className="text-lg font-semibold mb-2">Staff Panel</h3>
          <p className="text-gray-600">
            จัดการคำสั่งซื้อ, งานที่ได้รับมอบหมาย
          </p>
        </div>
      )}

      {user?.role === "user" && (
        <div className="bg-white rounded-xl shadow p-6 mb-4">
          <h3 className="text-lg font-semibold mb-2">User Panel</h3>
          <p className="text-gray-600">
            ดูข้อมูลส่วนตัว และประวัติการใช้งาน
          </p>
        </div>
      )}
    </div>
  );
}
