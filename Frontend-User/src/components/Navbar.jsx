// src/components/Navbar.jsx
import { Link } from "react-router-dom";
import { useCart } from "../context/CartContext";
import { ShoppingCart, UserRound, TentTree } from "lucide-react";

export default function Navbar() {
  const { cartItems } = useCart();
  return (
    <nav className="main-color-bg-brown text-white">
      <div className="container mx-auto flex justify-between items-center px-4 py-8">
        <Link to="/" className="text-4xl font-bold flex gap-5">
          <div>Hike-Cycle</div>
          <div className="text-xl self-end">เช่าอุปกรณ์เดินป่า</div>
        </Link>

        <div className="flex gap-8 text-xl">
          <div className="flex flex-col">
            <TentTree className="self-center" />
            <Link to="/products">สินค้า</Link>
          </div>
          <div className="flex flex-col">
            <ShoppingCart className="self-center" />
            <Link to="/cart">
              ตะกร้า
              {cartItems.length > 0 ? (
                <span className="ml-1 bg-red-700 text-white rounded-full px-2">
                  {cartItems.length}
                </span>
              ) : (
                ""
              )}
            </Link>
          </div>
          <div className="flex flex-col">
            <UserRound  className="self-center"/>
            <Link to="/login">เข้าสู่ระบบ</Link>
          </div>
        </div>
      </div>
    </nav>
  );
}
