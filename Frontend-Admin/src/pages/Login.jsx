import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

// mock user (ชั่วคราว)
import users from "../../data/user.json";

export default function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = (loginEmail = email, loginPassword = password) => {
    const foundUser = users.find(
      (u) => u.email === loginEmail && u.password === loginPassword,
    );

    if (!foundUser) {
      setError("Email หรือ Password ไม่ถูกต้อง");
      return;
    }

    // ข้อมูลที่เก็บใน auth + localStorage
    const userData = {
      name: foundUser.name,
      email: foundUser.email,
      phone: foundUser.phone,
      address: foundUser.address,
      role: foundUser.role, // user | admin | staff
    };

    console.log("Login success:", userData);

    login(userData);
    navigate("/"); // หรือ /products
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 px-4">
      <div className="bg-white w-full max-w-sm rounded-xl shadow p-6">
        <h1 className="text-2xl font-bold text-center mb-6">
          Hike-Cycle Login
        </h1>

        {error && (
          <div className="mb-4 text-sm text-red-600 bg-red-50 p-2 rounded">
            {error}
          </div>
        )}

        <div className="mb-4">
          <label className="block text-sm mb-1">Email</label>
          <input
            type="email"
            className="w-full border rounded px-3 py-2"
            placeholder="example@email.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>

        <div className="mb-6">
          <label className="block text-sm mb-1">Password</label>
          <input
            type="password"
            className="w-full border rounded px-3 py-2"
            placeholder="••••••••"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>

        <button
          onClick={() => handleLogin()}
          className="w-full bg-black text-white py-2 rounded hover:bg-gray-800 transition"
        >
          Login
        </button>
        <div className="flex justify-end gap-2 m-2">
          <button
            onClick={() => {
              handleLogin("a@gmail.com", "1");
            }}
            className="p-2 border-0 bg-amber-700 rounded-2xl"
          >
            admin
          </button>
          <button
            onClick={() => {
              handleLogin("s@gmail.com", "1");
            }}
            className="p-2 border-0 bg-emerald-500 rounded-2xl"
          >
            staff
          </button>
        </div>
      </div>
    </div>
  );
}
