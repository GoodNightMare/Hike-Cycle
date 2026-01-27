import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { UserRound, LogOut } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";
import users from "../data/user.json";

export default function Profile() {
  const navigate = useNavigate();
  const { user, updateProfile, logout } = useAuth();
  const [showModal, setShowModal] = useState(false);
  const [profileData, setProfileData] = useState(null);

  const [form, setForm] = useState({
    name: user?.name || "",
    phone: user?.phone || "",
    address: user?.address || "",
  });

  useEffect(() => {
    if (!user) navigate("/login");
  }, [user]);

  useEffect(() => {
    if (!user) {
      navigate("/login");
      return;
    }

    const foundUser = users.find((u) => u.email === user.email);

    if (foundUser) {
      setProfileData(foundUser);
      setForm({
        name: foundUser.name,
        phone: foundUser.phone,
        address: foundUser.address,
      });
    }
  }, [user]);

  const handleSave = () => {
    const payload = {
      ...profileData,
      name: form.name,
      phone: form.phone,
      address: form.address,
    };

    console.log("📦 UPDATE TO JSON:", payload);

    // ถ้าเป็น backend → ยิง API
    // ตอนนี้เป็น mock → update state
    setProfileData(payload);
    setShowModal(false);
  };

  const handleLogout = () => {
    logout(); // ลบ user + set state
    navigate("/login"); // redirect
  };

  return (
    <div className="min-h-screen">
      <div className="max-w-xl mx-auto bg-white p-8 rounded-xl shadow">
        <h1 className="text-2xl font-bold mb-6">👤 โปรไฟล์</h1>

        <div className="space-y-4 text-m">
          <div>
            <p className="text-gray-500">ชื่อ</p>
            <p className="font-semibold">{profileData?.name || "-"}</p>
          </div>

          <div>
            <p className="text-gray-500">Email</p>
            <p className="font-semibold">{user?.email || "-"}</p>
          </div>

          <div>
            <p className="text-gray-500">เบอร์โทร</p>
            <p className="font-semibold">{profileData?.phone || "-"}</p>
          </div>

          <div>
            <p className="text-gray-500">ที่อยู่</p>
            <p className="font-semibold">{profileData?.address || "-"}</p>
          </div>
        </div>

        <div className="flex justify-between">
          <button
            className="mt-6 bg-black text-white px-6 py-3 rounded-xl hover:bg-gray-800"
            onClick={() => setShowModal(true)}
          >
            แก้ไขข้อมูล
          </button>

          <div className="mt-6">
            <button
              onClick={handleLogout}
              className="
              h-full
                w-full flex items-center justify-center gap-2
                px-6 py-3 rounded-lg
                text-sm font-semibold
                text-white bg-red-500
                hover:bg-red-600
                active:scale-95
      transition-all
    "
            >
              <LogOut size={16} />
              ออกจากระบบ
            </button>
          </div>
        </div>
      </div>

      {/* ================= MODAL ================= */}
      {showModal && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white w-full max-w-md rounded-xl p-6">
            <h2 className="text-xl font-bold mb-4">แก้ไขโปรไฟล์</h2>

            <div className="space-y-3 text-sm">
              <div>
                <label className="block text-gray-500 mb-1">ชื่อ</label>
                <input
                  className="border w-full p-2 rounded"
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                />
              </div>

              <div>
                <label className="block text-gray-500 mb-1">Email</label>
                <input
                  className="border w-full p-2 rounded bg-gray-100"
                  value={user.email}
                  disabled
                />
              </div>

              <div>
                <label className="block text-gray-500 mb-1">เบอร์โทร</label>
                <input
                  className="border w-full p-2 rounded"
                  value={form.phone}
                  onChange={(e) => setForm({ ...form, phone: e.target.value })}
                />
              </div>

              <div>
                <label className="block text-gray-500 mb-1">ที่อยู่</label>
                <textarea
                  className="border w-full p-2 rounded"
                  rows={3}
                  value={form.address}
                  onChange={(e) =>
                    setForm({ ...form, address: e.target.value })
                  }
                />
              </div>
            </div>

            <div className="flex justify-end gap-2 mt-6">
              <button
                className="px-4 py-2 border rounded"
                onClick={() => setShowModal(false)}
              >
                ยกเลิก
              </button>
              <button
                className="px-4 py-2 bg-black text-white rounded"
                onClick={handleSave}
              >
                บันทึก
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
