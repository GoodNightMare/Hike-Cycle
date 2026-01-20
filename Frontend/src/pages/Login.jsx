// src/pages/Login.jsx
export default function Login() {
  return (
    <div className="max-w-sm mx-auto border p-4 rounded">
      <h1 className="text-xl font-bold mb-4">เข้าสู่ระบบ</h1>
      <input className="border w-full mb-2 p-2" placeholder="Email" />
      <input className="border w-full mb-4 p-2" type="password" placeholder="Password" />
      <button className="bg-black text-white w-full py-2">
        Login
      </button>
    </div>
  );
}
