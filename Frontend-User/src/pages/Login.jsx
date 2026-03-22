import { useState } from "react";
import user from "./../data/user.json";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import axios from "axios";

export default function Login() {
  const navigate = useNavigate();
  const { login: authLogin } = useAuth();

  const [users] = useState(user);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false); // ✅ เพิ่ม
  const [regError, setRegError] = useState("");

  const [showRegister, setShowRegister] = useState(false);

  const [regData, setRegData] = useState({
    name: "",
    email: "",
    password: "",
    phone: "",
    address: "",
  });

  const login = async (email, password) => {
    try {
      const response = await axios.post(
        "http://localhost:5279/api/auth/login",
        {
          email,
          password,
        },
      );

      const foundUser = response.data; // ข้อมูลที่ Backend ส่งกลับมา (เช่น id, email, role, fullName)

      const userData = {
        id: foundUser.id,
        email: foundUser.email,
        name: foundUser.fullName,
        role: foundUser.role,
        isLogin: true,
      };

      authLogin(userData);
      setError("");
      setSuccess(true);

      setTimeout(() => {
        navigate("/products");
      }, 1200);
    } catch (err) {
      setError(err.response?.data?.message || "Email หรือ Password ไม่ถูกต้อง");
      setSuccess(false);
    }
  };

  const isValidEmail = (email) => {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  };

  const isValidPhone = (phone) => {
    return /^0\d{9}$/.test(phone);
  };

const register = async () => {

  if (!isValidEmail(regData.email)) {
    setRegError("รูปแบบ Email ไม่ถูกต้อง");
    return;
  }
  if (regData.phone && !isValidPhone(regData.phone)) {
    setRegError("รูปแบบเบอร์โทรไม่ถูกต้อง (ต้องขึ้นต้นด้วย 0 และมี 10 หลัก)");
    return;
  }
    // Validation เบื้องต้น
    if (!regData.password) {
      setRegError("กรุณากรอก Password");
      return;
    }

    try {
      const payload = {
        email: regData.email,
        password: regData.password,
        fullName: regData.name,
        phone: regData.phone,
        address: regData.address,
      };

      await axios.post("http://localhost:5279/api/auth/register", payload);

      alert("สมัครสมาชิกสำเร็จ! กรุณาเข้าสู่ระบบ");
      setShowRegister(false);
      setRegError("");
      // ล้างข้อมูลฟอร์ม
      setRegData({ name: "", email: "", password: "", phone: "", address: "" });
    } catch (err) {
      setRegError(err.response?.data?.message || "ไม่สามารถสมัครสมาชิกได้");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center">
      {showRegister && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl w-full max-w-md p-6">
            {regError && (
              <div className="text-sm text-red-600 bg-red-50 p-2 rounded mb-3">
                {regError}
              </div>
            )}

            <h2 className="text-xl font-bold mb-4">สมัครสมาชิก</h2>

            <div className="space-y-3">
              <input
                placeholder="Email *"
                className="w-full border p-2 rounded"
                type="email"
                value={regData.email}
                onChange={(e) =>
                  setRegData({ ...regData, email: e.target.value })
                }
              />

              <input
                type="password"
                placeholder="Password *"
                className="w-full border p-2 rounded"
                value={regData.password}
                onChange={(e) =>
                  setRegData({ ...regData, password: e.target.value })
                }
              />

              <input
                placeholder="ชื่อ (ไม่บังคับ)"
                className="w-full border p-2 rounded"
                value={regData.name}
                onChange={(e) =>
                  setRegData({ ...regData, name: e.target.value })
                }
              />

              <input
                placeholder="เบอร์โทร (ไม่บังคับ)"
                className="w-full border p-2 rounded"
                maxLength={10}
                value={regData.phone}
                onChange={(e) => {
                  const value = e.target.value.replace(/\D/g, "");
                  setRegData({ ...regData, phone: value });
                }}
              />

              <textarea
                placeholder="ที่อยู่ (ไม่บังคับ)"
                className="w-full border p-2 rounded"
                value={regData.address}
                onChange={(e) =>
                  setRegData({ ...regData, address: e.target.value })
                }
              />
            </div>

            <div className="flex justify-end gap-2 mt-6">
              <button
                className="px-4 py-2 border rounded"
                onClick={() => setShowRegister(false)}
              >
                ยกเลิก
              </button>

              <button
                className="px-4 py-2 bg-black text-white rounded"
                onClick={register}
              >
                สมัครสมาชิก
              </button>
            </div>
          </div>
        </div>
      )}
      <div className="w-full max-w-sm bg-white p-6 rounded-xl shadow-lg">
        <h1 className="text-2xl font-bold text-center mb-1">เข้าสู่ระบบ</h1>
        <p className="text-sm text-gray-500 text-center mb-6">
          กรุณาเข้าสู่ระบบเพื่อใช้งาน
        </p>

        {error && (
          <div className="mb-4 text-sm text-red-600 bg-red-50 p-2 rounded">
            {error}
          </div>
        )}

        <div className="mb-4">
          <label className="block text-sm font-medium mb-1">Email</label>
          <input
            type="email"
            placeholder="example@email.com"
            className="w-full border rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-black"
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>

        <div className="mb-6">
          <label className="block text-sm font-medium mb-1">Password</label>
          <input
            type="password"
            placeholder="••••••••"
            className="w-full border rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-black"
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>

        {success && (
          <div className="mb-4 text-sm text-green-700 bg-green-100 p-2 rounded">
            ✅ ล็อกอินสำเร็จแล้ว
          </div>
        )}

        <button
          onClick={() => login(email, password)}
          className="w-full bg-black text-white py-2 rounded-lg hover:bg-gray-800 transition"
        >
          Login
        </button>

        <p className="text-center text-sm text-gray-500 mt-4">
          ยังไม่มีบัญชี?{" "}
          <span
            className="underline cursor-pointer text-gray-600 hover:text-red-500 transition-colors duration-500"
            onClick={() => setShowRegister(true)}
          >
            สมัครสมาชิก
          </span>
        </p>
      </div>
    </div>
  );
}
