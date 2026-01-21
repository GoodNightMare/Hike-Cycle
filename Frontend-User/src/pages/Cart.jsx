// src/pages/Cart.jsx
import { useEffect, useState } from "react";
import { useCart } from "../context/CartContext";
import user from "./../data/user.json";

export default function Cart() {
  const { cartItems, removeFromCart } = useCart();
  const [users, setUsers] = useState(user);
  const me = JSON.parse(localStorage.getItem("user"));
  
  const meInJson = users.find((u) => u.email === me?.email);

  const [showModal, setShowModal] = useState(false);
  const [deliveryType, setDeliveryType] = useState("address"); // address | store
  const [address, setAddress] = useState(meInJson?.address || "");


  useEffect(() => {
    console.log(users);
    console.log(meInJson);
  }, [users, meInJson]);

  if (cartItems.length === 0) {
    return (
      <div className="min-h-screen flex items-start justify-center">
        <div className="bg-white w-1/1 p-8 rounded-xl shadow text-center">
          <h1 className="text-2xl font-bold">ตะกร้าสินค้า</h1>
          <p className="text-gray-500 mt-2">ยังไม่มีสินค้าในตะกร้า</p>
        </div>
      </div>
    );
  }

  const calculateDays = (startDate, endDate) => {
    const start = new Date(startDate);
    const end = new Date(endDate);

    const diffTime = end - start;
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    return diffDays || 1; // ป้องกันกรณีวันเดียวกัน
  };

  const total = cartItems.reduce((sum, item) => {
    const days = calculateDays(item.startDate, item.endDate);
    return sum + item.product.price * days;
  }, 0);

  return (
    <div className="min-h-screen py-8">
      {showModal && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl w-full max-w-md p-6">
            <h2 className="text-xl font-bold mb-4">เลือกวิธีรับสินค้า</h2>

            {/* ตัวเลือก */}
            <div className="space-y-3">
              <label className="flex items-center gap-2">
                <input
                  type="radio"
                  checked={deliveryType === "address"}
                  onChange={() => setDeliveryType("address")}
                />
                ใช้ที่อยู่ที่มีอยู่
              </label>

              <label className="flex items-center gap-2">
                <input
                  type="radio"
                  checked={deliveryType === "store"}
                  onChange={() => setDeliveryType("store")}
                />
                ไปรับหน้าร้าน
              </label>
            </div>

            {/* กรอกที่อยู่ */}
            {deliveryType === "address" && (
              <textarea
                className="border w-full mt-4 p-2 rounded"
                rows={3}
                value={address}
                onChange={(e) => setAddress(e.target.value)}
                placeholder="กรอกที่อยู่จัดส่ง"
              />
            )}

            {/* ปุ่ม */}
            <div className="flex justify-end gap-2 mt-6">
              <button
                className="px-4 py-2 border rounded"
                onClick={() => setShowModal(false)}
              >
                ยกเลิก
              </button>

              <button
                className="px-4 py-2 bg-black text-white rounded"
                onClick={() => {
                  const orderData = {
                    items: cartItems,
                    total,
                    deliveryType,
                    address:
                      deliveryType === "address" ? address : "รับหน้าร้าน",
                  };

                  console.log("ORDER:", orderData);
                  setShowModal(false);
                }}
              >
                ยืนยัน
              </button>
            </div>
          </div>
        </div>
      )}

      <div className="max-w-5xl mx-auto p-6 bg-white rounded-xl shadow">
        <h1 className="text-2xl font-bold mb-6 flex items-center gap-2">
          🛒 ตะกร้าสินค้า
        </h1>

        <div className="space-y-4">
          {cartItems.map((item, index) => {
            const days = calculateDays(item.startDate, item.endDate);

            return (
              <div
                key={index}
                className="flex flex-col sm:flex-row gap-4 border rounded-lg p-4 hover:shadow transition"
              >
                <img
                  src={item.product.images[0]}
                  className="w-24 h-24 object-contain bg-gray-50 rounded"
                />

                <div className="flex-1 text-sm">
                  <h2 className="font-semibold text-base mb-2">
                    {item.product.name}
                  </h2>

                  {item.size && (
                    <p className="text-gray-600">ไซส์: {item.size}</p>
                  )}

                  <p className="text-gray-600 my-2">
                    วันที่เช่า: {item.startDate} → {item.endDate}
                  </p>

                  <div className="flex gap-4">
                    <p className="text-gray-600">เวลารับ: All</p>
                    <p className="text-gray-600">เวลาคืน: {item.time}</p>
                  </div>
                </div>

                <div className="text-right min-w-[140px]">
                  <p className="text-sm text-gray-500">
                    {days} วัน × ฿{item.product.price.toLocaleString()}
                  </p>

                  <p className="font-bold text-green-600 text-lg">
                    ฿{(item.product.price * days).toLocaleString()}
                  </p>

                  <button
                    onClick={() => removeFromCart(index)}
                    className="text-red-500 text-sm mt-3 hover:underline"
                  >
                    ลบสินค้า
                  </button>
                </div>
              </div>
            );
          })}
        </div>

        {/* ✅ ราคารวม */}
        <div className="mt-8 border-t pt-6 flex justify-between items-center text-lg font-bold">
          <span>รวมทั้งหมด</span>
          <span className="text-green-600 text-2xl">
            ฿{total.toLocaleString()}
          </span>
        </div>

        <div className="flex justify-end mt-6">
          <button
            className="bg-black hover:bg-gray-800 text-white px-8 py-4 rounded-xl text-lg font-semibold transition"
            onClick={() => setShowModal(true)}
          >
            ชำระเงิน
          </button>
        </div>
      </div>
    </div>
  );
}
