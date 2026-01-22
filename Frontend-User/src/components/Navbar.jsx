// src/components/Navbar.jsx
import { Link, useNavigate } from "react-router-dom";
import { useCart } from "../context/CartContext";
import { ShoppingCart, UserRound, TentTree, LogOut } from "lucide-react";
import { useAuth } from "../context/AuthContext";

export default function Navbar() {
  const { user, logout } = useAuth(); // ✅ ใช้จาก context อย่างเดียว
  const { cartItems } = useCart();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();           // ลบ user + set state
    navigate("/login"); // redirect
  };

  return (
    <nav className="main-color-bg-brown text-white">
      <div className="container mx-auto flex justify-between items-center px-4 py-8">
        <Link to="/" className="text-4xl font-bold flex gap-5">
          <div>Hike-Cycle</div>
          <div className="text-xl self-end">เช่าอุปกรณ์เดินป่า</div>
        </Link>

        <div className="flex gap-8 text-xl">
          {/* สินค้า */}
          <div className="flex flex-col items-center">
            <TentTree />
            <Link to="/products">สินค้า</Link>
          </div>

          {/* ตะกร้า */}
          <div className="flex flex-col items-center">
            <ShoppingCart />
            <Link to="/cart">
              ตะกร้า
              {cartItems.length > 0 && (
                <span className="ml-1 bg-red-700 text-white rounded-full px-2 text-sm">
                  {cartItems.length}
                </span>
              )}
            </Link>
          </div>

          {/* Login / Profile */}
          {!user ? (
            <div className="flex flex-col items-center">
              <UserRound />
              <Link to="/login">เข้าสู่ระบบ</Link>
            </div>
          ) : (
            <div className="flex flex-col items-center gap-1">
              <UserRound />
              <Link to="/profile">โปรไฟล์</Link>
              <button
                onClick={handleLogout}
                className="text-sm text-red-300 hover:text-red-500 flex items-center gap-1"
              >
                <LogOut size={16} />
                ออกจากระบบ
              </button>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
}
