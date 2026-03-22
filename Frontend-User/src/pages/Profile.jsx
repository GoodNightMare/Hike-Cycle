import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { LogOut } from "lucide-react";
import { useNavigate } from "react-router-dom";
import axios from "axios"; // ✅ ใช้ axios แทนการ import json

export default function Profile() {
  const navigate = useNavigate();
  const { user, logout } = useAuth(); // ดึง user จาก context (ที่มี id, email)
  const [showModal, setShowModal] = useState(false);
  const [loading, setLoading] = useState(true);

  const [form, setForm] = useState({
    name: "",
    phone: "",
    address: "",
  });

  // 1. ดึงข้อมูลโปรไฟล์จาก Backend เมื่อเข้าหน้านี้
  useEffect(() => {
    if (!user) {
      navigate("/login");
      return;
    }

    const fetchProfile = async () => {
      try {
        // ใช้ user.id ที่ได้มาตอน Login เพื่อดึงโปรไฟล์
        const response = await axios.get(`http://localhost:5279/api/auth/profile/${user.id}`);
        const data = response.data;

        setForm({
          name: data.fullName || "",
          phone: data.phone || "",
          address: data.address || "",
        });
        setLoading(false);
      } catch (error) {
        console.error("Error fetching profile:", error);
        setLoading(false);
      }
    };

    fetchProfile();
  }, [user, navigate]);

  // 2. ฟังก์ชันบันทึกข้อมูลลง Database
  const handleSave = async () => {
    try {
      const payload = {
        userId: user.id,
        fullName: form.name,
        phone: form.phone,
        address: form.address,
      };

      await axios.put(`http://localhost:5279/api/auth/profile/update`, payload);
      
      alert("อัปเดตข้อมูลสำเร็จ!");
      setShowModal(false);
      // อาจจะสั่ง window.location.reload() หรือ update context ถ้าจำเป็น
    } catch (error) {
      console.error("Update error:", error);
      alert("ไม่สามารถบันทึกข้อมูลได้");
    }
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  if (loading) return <div className="text-center py-10">กำลังโหลดข้อมูล...</div>;

  return (
    <div className="min-h-screen py-10 bg-gray-50">
      <div className="max-w-xl mx-auto bg-white p-8 rounded-xl shadow">
        <h1 className="text-2xl font-bold mb-6">👤 โปรไฟล์</h1>

        <div className="space-y-4 text-m border-b pb-6">
          <div>
            <p className="text-gray-400 text-sm">ชื่อ-นามสกุล</p>
            <p className="font-semibold text-lg">{form.name || "-"}</p>
          </div>

          <div>
            <p className="text-gray-400 text-sm">อีเมล (แก้ไขไม่ได้)</p>
            <p className="font-semibold text-gray-600">{user?.email || "-"}</p>
          </div>

          <div>
            <p className="text-gray-400 text-sm">เบอร์โทรศัพท์</p>
            <p className="font-semibold">{form.phone || "-"}</p>
          </div>

          <div>
            <p className="text-gray-400 text-sm">ที่อยู่จัดส่ง</p>
            <p className="font-semibold">{form.address || "-"}</p>
          </div>
        </div>

        <div className="flex justify-between mt-6">
          <button
            className="bg-black text-white px-6 py-2 rounded-lg hover:bg-gray-800 transition"
            onClick={() => setShowModal(true)}
          >
            แก้ไขโปรไฟล์
          </button>

          <button
            onClick={handleLogout}
            className="flex items-center gap-2 px-6 py-2 rounded-lg text-white bg-red-500 hover:bg-red-600 transition"
          >
            <LogOut size={16} />
            ออกจากระบบ
          </button>
        </div>
      </div>

      {/* Modal แก้ไขข้อมูล เหมือนเดิมแต่ใช้ handleSave ใหม่ */}
      {showModal && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50 p-4">
          <div className="bg-white w-full max-w-md rounded-xl p-6 shadow-2xl">
            <h2 className="text-xl font-bold mb-4">แก้ไขข้อมูลส่วนตัว</h2>
            <div className="space-y-4">
              <div>
                <label className="block text-sm text-gray-500 mb-1">ชื่อ-นามสกุล</label>
                <input
                  className="border w-full p-2 rounded-lg focus:ring-2 focus:ring-black outline-none"
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                />
              </div>
              <div>
                <label className="block text-sm text-gray-500 mb-1">เบอร์โทรศัพท์</label>
                <input
                  className="border w-full p-2 rounded-lg focus:ring-2 focus:ring-black outline-none"
                  value={form.phone}
                  onChange={(e) => setForm({ ...form, phone: e.target.value })}
                />
              </div>
              <div>
                <label className="block text-sm text-gray-500 mb-1">ที่อยู่</label>
                <textarea
                  className="border w-full p-2 rounded-lg focus:ring-2 focus:ring-black outline-none"
                  rows={3}
                  value={form.address}
                  onChange={(e) => setForm({ ...form, address: e.target.value })}
                />
              </div>
            </div>
            <div className="flex justify-end gap-3 mt-8">
              <button className="px-4 py-2 text-gray-500" onClick={() => setShowModal(false)}>ยกเลิก</button>
              <button className="px-6 py-2 bg-black text-white rounded-lg" onClick={handleSave}>บันทึกข้อมูล</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}