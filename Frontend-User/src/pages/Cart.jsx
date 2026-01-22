// src/pages/Cart.jsx
import { useEffect, useState } from "react";
import { useCart } from "../context/CartContext";
import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";

export default function Cart() {
  const { cartItems, removeFromCart } = useCart();
  const { user } = useAuth();
  const navigate = useNavigate();

  const [showModal, setShowModal] = useState(false);
  const [deliveryType, setDeliveryType] = useState("address");
  const [address, setAddress] = useState("");

  // ❌ ยังไม่ login
  if (!user) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="bg-white p-6 rounded-xl shadow text-center">
          <h2 className="text-xl font-bold mb-2">กรุณาเข้าสู่ระบบ</h2>
          <p className="text-gray-500 mb-4">
            ต้องเข้าสู่ระบบก่อนจึงจะใช้งานตะกร้าได้
          </p>
          <button
            onClick={() => navigate("/login")}
            className="bg-black text-white px-4 py-2 rounded"
          >
            เข้าสู่ระบบ
          </button>
        </div>
      </div>
    );
  }

  if (cartItems.length === 0) {
    return (
      <div className="min-h-screen flex items-start justify-center">
        <div className="bg-white p-8 w-full rounded-xl shadow text-center">
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
    return Math.max(1, Math.ceil(diffTime / (1000 * 60 * 60 * 24)));
  };

  const total = cartItems.reduce((sum, item) => {
    const days = calculateDays(item.startDate, item.endDate);
    return sum + item.product.price * days;
  }, 0);

  const handleCheckout = async () => {
    // 🔒 กันกรณีข้อมูลไม่ครบ
    if (deliveryType === "address" && !address.trim()) {
      alert("กรุณากรอกที่อยู่จัดส่ง");
      return;
    }

    // 🧾 ข้อมูล Order (เตรียมส่ง Backend)
    const orderPayload = {
      user: {
        email: user.email,
        id: user.id || null, // เผื่อมีในอนาคต
      },
      items: cartItems.map((item) => ({
        productId: item.product.id,
        name: item.product.name,
        price: item.product.price,
        size: item.size || null,
        startDate: item.startDate,
        endDate: item.endDate,
      })),
      delivery: {
        type: deliveryType, // "address" | "store"
        address: deliveryType === "address" ? address : "รับหน้าร้าน",
      },
      totalPrice: total,
      paymentStatus: "PENDING", // READY | PAID | FAILED
      createdAt: new Date().toISOString(),
    };

    try {
      console.log("📦 ORDER PAYLOAD:", orderPayload);

      // 🔜 ของจริงในอนาคต
      // await api.post("/orders", orderPayload);

      alert("เตรียมเข้าสู่หน้าชำระเงินจริง");
      setShowModal(false);

      // 🔜 หลังชำระเงินสำเร็จ
      // clearCart();
      // navigate("/order-success");
    } catch (error) {
      console.error("Checkout error:", error);
      alert("เกิดข้อผิดพลาดในการชำระเงิน");
    }
  };

  return (
    <div className="min-h-screen py-8">
      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl w-full max-w-md p-6">
            <h2 className="text-xl font-bold mb-4">เลือกวิธีรับสินค้า</h2>

            <div className="space-y-3">
              <label className="flex items-center gap-2">
                <input
                  type="radio"
                  checked={deliveryType === "address"}
                  onChange={() => setDeliveryType("address")}
                />
                จัดส่งตามที่อยู่
              </label>

              <label className="flex items-center gap-2">
                <input
                  type="radio"
                  checked={deliveryType === "store"}
                  onChange={() => setDeliveryType("store")}
                />
                รับหน้าร้าน
              </label>
            </div>

            {deliveryType === "address" && (
              <textarea
                className="border w-full mt-4 p-2 rounded"
                rows={3}
                value={address}
                onChange={(e) => setAddress(e.target.value)}
                placeholder="กรอกที่อยู่จัดส่ง"
              />
            )}

            <div className="flex justify-end gap-2 mt-6">
              <button
                className="px-4 py-2 border rounded"
                onClick={() => setShowModal(false)}
              >
                ยกเลิก
              </button>

              <button
                disabled={deliveryType === "address" && !address.trim()}
                className="px-4 py-2 bg-black text-white rounded disabled:bg-gray-400"
                onClick={() => {
                  handleCheckout();
                }}
              >
                ยืนยัน
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Cart List */}
      <div className="max-w-5xl mx-auto p-6 bg-white rounded-xl shadow">
        <h1 className="text-2xl font-bold mb-6">🛒 ตะกร้าสินค้า</h1>

        <div className="space-y-4">
          {cartItems.map((item, index) => {
            const days = calculateDays(item.startDate, item.endDate);

            return (
              <div
                key={index}
                className="flex flex-col sm:flex-row gap-4 border rounded-lg p-4"
              >
                <img
                  src={item.product.images[0]}
                  className="w-24 h-24 object-contain bg-gray-50 rounded"
                />

                <div className="flex-1 text-sm">
                  <h2 className="font-semibold text-base mb-2">
                    {item.product.name}
                  </h2>
                  {item.size && <p>ไซส์: {item.size}</p>}
                  <p>
                    วันที่เช่า: {item.startDate} → {item.endDate}
                  </p>
                </div>

                <div className="text-right min-w-[140px]">
                  <p className="text-sm text-gray-500">
                    {days} วัน × ฿{item.product.price}
                  </p>
                  <p className="font-bold text-green-600 text-lg">
                    ฿{item.product.price * days}
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

        <div className="mt-8 border-t pt-6 flex justify-between items-center">
          <span className="font-bold">รวมทั้งหมด</span>
          <span className="text-green-600 text-2xl font-bold">
            ฿{total.toLocaleString()}
          </span>
        </div>

        <div className="flex justify-end mt-6">
          <button
            onClick={() => setShowModal(true)}
            className="bg-black text-white px-8 py-4 rounded-xl text-lg font-semibold"
          >
            ชำระเงิน
          </button>
        </div>
      </div>
    </div>
  );
}
