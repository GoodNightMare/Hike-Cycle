import { useEffect, useState } from "react";
import user from "./../data/user.json";
import { useNavigate } from "react-router-dom";

// src/pages/Login.jsx
export default function Login() {
  const navigate = useNavigate();

  const [users, setUsers] = useState(user);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const login = (email, password) => {
    const user = users.find(
      (u) => u.email === email && u.password === password,
    );

    if (!user) {
      return {
        success: false,
        message: "Email หรือ Password ไม่ถูกต้อง",
      };
    }

    const userData = {
      email: user.email,
      role: user.role,
      isLogin: true,
    };

    localStorage.setItem("user", JSON.stringify(userData));

    alert("Login สำเร็จ");
    console.log("User Login:", userData);
    navigate("/products");
  };

  return (
    <div className="max-w-sm mx-auto border p-4 rounded">
      <h1 className="text-xl font-bold mb-4">เข้าสู่ระบบ</h1>
      <input
        className="border w-full mb-2 p-2"
        onChange={(e) => setEmail(e.target.value)}
        placeholder="Email"
      />
      <input
        className="border w-full mb-4 p-2"
        onChange={(e) => setPassword(e.target.value)}
        type="password"
        placeholder="Password"
      />
      <button
        className="bg-black text-white w-full py-2"
        onClick={() => login(email, password)}
      >
        Login
      </button>
    </div>
  );
}
