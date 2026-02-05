import { useState } from "react";
import productsData from "../../../Frontend-User/src/data/products.json";

export default function ProductsPage() {
  const [products, setProducts] = useState(productsData);
  const [selectedCategory, setSelectedCategory] = useState("all");
  const [showModal, setShowModal] = useState(false);
  const [mode, setMode] = useState("add"); // add | edit
  const [editingId, setEditingId] = useState(null);

  const emptyForm = {
    id: "",
    name: "",
    category: "",
    brand: "",
    price: "",
    stock: "",
    status: "active",
    description: ""
  };

  const [form, setForm] = useState(emptyForm);

  /* ================= FILTER ================= */
  const categories = [
    "all",
    ...new Set(products.map((p) => p.category))
  ];

  const filteredProducts =
    selectedCategory === "all"
      ? products
      : products.filter((p) => p.category === selectedCategory);

  /* ================= HANDLERS ================= */
  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  };

  const openAddModal = () => {
    setMode("add");
    setEditingId(null);
    setForm(emptyForm);
    setShowModal(true);
  };

  const openEditModal = (product) => {
    setMode("edit");
    setEditingId(product.id);
    setForm({
      id: product.id,
      name: product.name,
      category: product.category,
      brand: product.brand,
      price: product.price,
      stock: product.stock ?? "",
      status: product.status,
      description: product.description
    });
    setShowModal(true);
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (mode === "add") {
      console.log("POST /api/products", form);
      setProducts([...products, form]);
    } else {
      console.log("PUT /api/products/" + editingId, form);
      setProducts(
        products.map((p) =>
          p.id === editingId ? { ...p, ...form } : p
        )
      );
    }

    setShowModal(false);
  };

  /* ================= UI ================= */
  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">จัดการสินค้า</h1>
        <button
          onClick={openAddModal}
          className="px-4 py-2 bg-gray-800 text-white rounded-lg"
        >
          เพิ่มสินค้า
        </button>
      </div>

      {/* Category Filter */}
      <div className="flex gap-2 mb-6 flex-wrap">
        {categories.map((c) => (
          <button
            key={c}
            onClick={() => setSelectedCategory(c)}
            className={`px-4 py-2 rounded-lg border ${
              selectedCategory === c
                ? "bg-gray-900 text-white"
                : "border-gray-400"
            }`}
          >
            {c.toUpperCase()}
          </button>
        ))}
      </div>

      {/* TABLE */}
      <div className="overflow-x-auto bg-white shadow rounded-lg">
        <table className="w-full border">
          <thead className="bg-gray-100">
            <tr>
              <th className="border px-3 py-2">ID</th>
              <th className="border px-3 py-2">ชื่อ</th>
              <th className="border px-3 py-2">หมวด</th>
              <th className="border px-3 py-2">แบรนด์</th>
              <th className="border px-3 py-2">ราคา/วัน</th>
              <th className="border px-3 py-2">Stock</th>
              <th className="border px-3 py-2">Status</th>
              <th className="border px-3 py-2">Action</th>
            </tr>
          </thead>

          <tbody>
            {filteredProducts.map((p) => (
              <tr key={p.id} className="hover:bg-gray-50">
                <td className="border px-3 py-2">{p.id}</td>
                <td className="border px-3 py-2">{p.name}</td>
                <td className="border px-3 py-2">{p.category}</td>
                <td className="border px-3 py-2">{p.brand}</td>
                <td className="border px-3 py-2">{p.price} ฿</td>

                <td className="border px-3 py-2">
                  {p.variants
                    ? p.variants.reduce(
                        (sum, v) => sum + v.stock,
                        0
                      )
                    : p.stock}
                </td>

                <td className="border px-3 py-2">
                  <span
                    className={`px-2 py-1 rounded text-sm ${
                      p.status === "active"
                        ? "bg-green-100 text-green-700"
                        : "bg-red-100 text-red-700"
                    }`}
                  >
                    {p.status}
                  </span>
                </td>

                <td className="border px-3 py-2">
                  <button
                    onClick={() => openEditModal(p)}
                    className="px-3 py-1 bg-yellow-500 text-white rounded text-sm"
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
          <div className="bg-white w-full max-w-lg rounded-lg p-6">
            <h2 className="text-xl font-bold mb-4">
              {mode === "add" ? "เพิ่มสินค้า" : "แก้ไขสินค้า"}
            </h2>

            <form onSubmit={handleSubmit} className="space-y-4">
              <input
                name="id"
                placeholder="Product ID"
                value={form.id}
                onChange={handleChange}
                disabled={mode === "edit"}
                className="w-full border px-3 py-2 rounded"
                required
              />

              <input
                name="name"
                placeholder="ชื่อสินค้า"
                value={form.name}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
                required
              />

              <input
                name="category"
                placeholder="หมวดสินค้า"
                value={form.category}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
              />

              <input
                name="brand"
                placeholder="แบรนด์"
                value={form.brand}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
              />

              <input
                type="number"
                name="price"
                placeholder="ราคา / วัน"
                value={form.price}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
              />

              <input
                type="number"
                name="stock"
                placeholder="จำนวนคงเหลือ (ถ้าไม่มี variants)"
                value={form.stock}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
              />

              <select
                name="status"
                value={form.status}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
              >
                <option value="active">Active</option>
                <option value="inactive">Inactive</option>
              </select>

              <textarea
                name="description"
                placeholder="รายละเอียดสินค้า"
                value={form.description}
                onChange={handleChange}
                className="w-full border px-3 py-2 rounded"
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
                  Save
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
