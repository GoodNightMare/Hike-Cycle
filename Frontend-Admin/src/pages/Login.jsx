import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import axios from "axios"; // ✅ ใช้ axios แทน json

export default function Login() {
  const navigate = useNavigate();
  const { login: authLogin } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false); // ✅ เพิ่มสถานะโหลด

  // 🛡️ ฟังก์ชันหลักในการ Login
  const handleLogin = async (loginEmail = email, loginPassword = password) => {
    if (!loginEmail || !loginPassword) {
      setError("กรุณากรอกอีเมลและรหัสผ่าน");
      return;
    }

    setLoading(true);
    setError("");

    try {
      // ✅ ส่ง Request ไปยัง AuthController.cs
      const response = await axios.post("http://localhost:5279/api/auth/login", {
        email: loginEmail,
        password: loginPassword, // ส่งรหัสปกติไป เดี๋ยว Backend จะ Hash เป็น SHA256 ให้เอง
      });

      const foundUser = response.data;

      // ✅ จัดข้อมูลให้ตรงกับที่ AuthContext และหน้า Profile ต้องการ
      const userData = {
        id: foundUser.id,
        email: foundUser.email,
        name: foundUser.fullName, // มาจากตาราง user_profiles
        role: foundUser.role,     // user | admin
        isLogin: true,
      };

      authLogin(userData); // เก็บลง Context / LocalStorage
      navigate("/");       // ไปหน้าหลัก
    } catch (err) {
      console.error("Login Error:", err);
      // แสดงข้อความ Error จาก Backend (ถ้ามี)
      setError(err.response?.data?.message || "Email หรือ Password ไม่ถูกต้อง");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 px-4">
      <div className="bg-white w-full max-w-sm rounded-xl shadow-lg p-6">
        <h1 className="text-2xl font-bold text-center mb-2">Hike-Cycle</h1>
        <p className="text-center text-gray-500 text-sm mb-6">กรุณาเข้าสู่ระบบเพื่อใช้งาน</p>

        {error && (
          <div className="mb-4 text-sm text-red-600 bg-red-50 p-3 rounded-lg border border-red-100">
            {error}
          </div>
        )}

        <div className="mb-4">
          <label className="block text-sm font-medium mb-1">Email</label>
          <input
            type="email"
            className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-black outline-none transition"
            placeholder="example@email.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>

        <div className="mb-6">
          <label className="block text-sm font-medium mb-1">Password</label>
          <input
            type="password"
            className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-black outline-none transition"
            placeholder="••••••••"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>

        <button
          onClick={() => handleLogin()}
          disabled={loading}
          className="w-full bg-black text-white py-2 rounded-lg hover:bg-gray-800 transition disabled:bg-gray-400"
        >
          {loading ? "กำลังเข้าสู่ระบบ..." : "Login"}
        </button>

        {/* ปุ่มลัดสำหรับ Test ข้อมูลใน DB จริงที่คุณ Insert ไว้ */}
        <div className="mt-6 pt-4 border-t border-gray-100">
          <p className="text-center text-xs text-gray-400 mb-3">Quick Login (Test Data)</p>
          <div className="flex justify-center gap-2">
            <button
              onClick={() => handleLogin("admin1@hikecycle.com", "password123")}
              className="px-3 py-1 text-xs bg-amber-600 text-white rounded-full hover:bg-amber-700"
            >
              Admin
            </button>
            <button
              onClick={() => handleLogin("staff1@gmail.com", "password123")}
              className="px-3 py-1 text-xs bg-emerald-600 text-white rounded-full hover:bg-emerald-700"
            >
              Staff
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}