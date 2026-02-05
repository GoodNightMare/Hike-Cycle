import { useState } from "react";
import usersData from "../../data/user.json";

export default function UsersPage() {
  const [users, setUsers] = useState(usersData);
  const [selectedRole, setSelectedRole] = useState("all");
  const [showModal, setShowModal] = useState(false);

  const [mode, setMode] = useState("add"); // add | edit
  const [editingId, setEditingId] = useState(null);

  const today = new Date().toISOString().split("T")[0];

  const [form, setForm] = useState({
    email: "",
    name: "",
    password: "",
    phone: "",
    address: "",
    role: "user",
    createdAt: today
  });

  /* ================= FILTER ================= */
  const filteredUsers =
    selectedRole === "all"
      ? users
      : users.filter((u) => u.role === selectedRole);

  /* ================= HANDLERS ================= */
  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  };

  const openAddModal = () => {
    setMode("add");
    setEditingId(null);
    setForm({
      email: "",
      name: "",
      password: "",
      phone: "",
      address: "",
      role: "user",
      createdAt: today
    });
    setShowModal(true);
  };

  const handleEdit = (user) => {
    setMode("edit");
    setEditingId(user.id);
    setForm({
      email: user.email,
      name: user.name,
      password: "",
      phone: user.phone,
      address: user.address,
      role: user.role,
      createdAt: user.createdAt
    });
    setShowModal(true);
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (mode === "add") {
      const newUser = {
        id: users.length + 1,
        ...form
      };

      console.log("POST /api/users 👉", newUser);
      // axios.post("/api/users", newUser)

      setUsers([...users, newUser]);
    } else {
      const updatedUsers = users.map((u) =>
        u.id === editingId ? { ...u, ...form } : u
      );

      console.log("PUT /api/users/" + editingId, form);
      // axios.put(`/api/users/${editingId}`, form)

      setUsers(updatedUsers);
    }

    setShowModal(false);
  };

  /* ================= UI ================= */
  return (
    <div className="p-6">
      {/* Add Button */}
      <button
        onClick={openAddModal}
        className="mb-6 px-4 py-2 bg-gray-700 text-white rounded-lg"
      >
        เพิ่มผู้ใช้งาน
      </button>

      <h1 className="text-2xl font-bold mb-4">จัดการผู้ใช้งาน</h1>

      {/* Role Filter */}
      <div className="flex gap-3 mb-6">
        {["all", "user", "staff", "admin"].map((r) => (
          <button
            key={r}
            onClick={() => setSelectedRole(r)}
            className={`px-4 py-2 rounded-lg border ${
              selectedRole === r
                ? "bg-gray-600 text-white"
                : "border-gray-400"
            }`}
          >
            {r.toUpperCase()}
          </button>
        ))}
      </div>

      {/* Table */}
      <div className="overflow-x-auto shadow rounded-lg">
        <table className="w-full border bg-white">
          <thead className="bg-gray-100">
            <tr>
              <th className="border px-4 py-2">ID</th>
              <th className="border px-4 py-2">ชื่อ</th>
              <th className="border px-4 py-2">Email</th>
              <th className="border px-4 py-2">เบอร์</th>
              <th className="border px-4 py-2">ที่อยู่</th>
              <th className="border px-4 py-2">Role</th>
              <th className="border px-4 py-2">วันที่สมัคร</th>
              <th className="border px-4 py-2">สถานะ</th>
              <th className="border px-4 py-2">Action</th>
            </tr>
          </thead>
          <tbody>
            {filteredUsers.map((u) => (
              <tr key={u.id} className="hover:bg-gray-50">
                <td className="border px-4 py-2">{u.id}</td>
                <td className="border px-4 py-2">{u.name}</td>
                <td className="border px-4 py-2">{u.email}</td>
                <td className="border px-4 py-2">{u.phone}</td>
                <td className="border px-4 py-2">{u.address}</td>
                <td className="border px-4 py-2">{u.role}</td>
                <td className="border px-4 py-2">
                  <span
                    className={`px-2 py-1 rounded text-sm ${
                      u.isActived ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"
                    }`}
                  >
                    {u.isActived ? "Active" : "Inactive"}
                  </span>
                </td>
                <td className="border px-4 py-2">{u.createdAt}</td>
                <td className="border px-4 py-2">
                  <button
                    onClick={() => handleEdit(u)}
                    className="px-3 py-1 text-sm bg-yellow-500 text-white rounded"
                  >
                    แก้ไข
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* ================= MODAL ================= */}
      {showModal && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg w-full max-w-lg p-6">
            <h2 className="text-xl font-bold mb-4">
              {mode === "add" ? "เพิ่มผู้ใช้งาน" : "แก้ไขผู้ใช้งาน"}
            </h2>

            <form onSubmit={handleSubmit} className="space-y-4">
              <input
                name="email"
                placeholder="Email"
                value={form.email}
                onChange={handleChange}
                disabled={mode === "edit"}
                className="w-full border px-3 py-2 rounded"
                required
              />

              <input
                type="password"
                name="password"
                placeholder="รหัสผ่าน"
                value={form.password}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
                required={mode === "add"}
              />

              <input
                name="name"
                placeholder="ชื่อ"
                value={form.name}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
              />

              <input
                name="phone"
                placeholder="เบอร์โทร"
                value={form.phone}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
              />

              <textarea
                name="address"
                placeholder="ที่อยู่"
                value={form.address}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
              />

              <select
                name="role"
                value={form.role}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
              >
                <option value="user">User</option>
                <option value="staff">Staff</option>
                <option value="admin">Admin</option>
              </select>

              <select
                name="isActived"
                value={form.isActived}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
              >
                <option value="true">Active</option>
                <option value="false">Inactive</option>
              </select>

              <input
                type="date"
                value={form.createdAt}
                disabled
                className="w-full border px-3 py-2 rounded bg-gray-100"
              />

              <div className="flex justify-end gap-3 pt-4">
                <button
                  type="button"
                  onClick={() => setShowModal(false)}
                  className="px-4 py-2 border rounded-lg"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-green-600 text-white rounded-lg"
                >
                  {mode === "add" ? "Save" : "Update"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
