import { useParams } from "react-router-dom";
import products from "../data/products.json";
import { Star, Check, X } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCart } from "../context/CartContext";

export default function ProductDetail() {
  const navigate = useNavigate();
  const { addToCart } = useCart();

  const user = JSON.parse(localStorage.getItem("user"));

  const tomorrow = new Date();
  const afterTomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  afterTomorrow.setDate(tomorrow.getDate() + 2);
  const minDate = tomorrow.toISOString().split("T")[0];

  const [blink, setBlink] = useState(false);

  const [showLoginModal, setShowLoginModal] = useState(false);

  const [startDate, setStartDate] = useState(
    tomorrow.toISOString().split("T")[0],
  );
  const [endDate, setEndDate] = useState(
    afterTomorrow.toISOString().split("T")[0],
  );
  const [time, setTime] = useState("08:00");

  const [openModal, setOpenModal] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);

  const [image, setImage] = useState(null);
  const [selectedSize, setSelectedSize] = useState("");
  const { id } = useParams();
  const product = products.find((p) => p.id === id);

  const isValidTime = time >= "08:00" && time <= "20:00";

  const totalStock =
    product?.category === "shoes" && product?.variants
      ? product.variants.reduce((sum, v) => sum + v.stock, 0)
      : (product?.stock ?? 0);

  const handleBlink = () => {
    setBlink(true);
    setTimeout(() => setBlink(false), 800);
  };

  useEffect(() => {
    setImage(product.images[0]);
  }, [product]);

  if (!product) {
    return <p className="text-center mt-10">ไม่พบสินค้า</p>;
  }

  return (
    <div className=" min-h-screen">
      <div className="max-w-6xl mx-auto p-6 bg-white rounded-xl shadow grid md:grid-cols-2 gap-8 relative">
        {blink && <div className="blink-screen" />}
        {openModal && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 w-full max-w-md">
              <h2 className="text-xl font-bold mb-4">
                จองสินค้า: {selectedProduct?.name}
              </h2>

              <div className="space-y-4">
                <div>
                  <label className="block text-sm mb-1">วันที่เริ่มเช่า</label>
                  <input
                    type="date"
                    min={minDate}
                    className="border p-2 rounded w-full"
                    value={startDate}
                    onChange={(e) => setStartDate(e.target.value)}
                  />
                </div>

                <div>
                  <label className="block text-sm mb-1">วันที่คืน</label>
                  <input
                    type="date"
                    min={startDate || minDate}
                    className="border p-2 rounded w-full"
                    value={endDate}
                    onChange={(e) => setEndDate(e.target.value)}
                  />
                </div>

                <div className="flex flex-col">
                  <div className="flex justify-between">
                    <label className="block text-sm mb-1">เวลาคืน</label>{" "}
                    {!isValidTime && (
                      <span className="text-red-500 text-sm">
                        กรุณาจองเวลาตั้งแต่ 08:00 ถึง 20:00
                      </span>
                    )}
                  </div>
                  <input
                    type="time"
                    min="08:00"
                    max="20:00"
                    className="border p-2 rounded w-full"
                    value={time}
                    onChange={(e) => setTime(e.target.value)}
                  />
                </div>
              </div>

              <div className="flex justify-end gap-2 mt-6">
                <button
                  onClick={() => {
                    setOpenModal(false);
                    setStartDate("");
                    setEndDate("");
                    setTime("");
                  }}
                  className="px-4 py-2 rounded border"
                >
                  ยกเลิก
                </button>

                <button
                  disabled={!startDate || !endDate || !time || !isValidTime}
                  className="px-4 py-2 rounded bg-black text-white disabled:bg-gray-400"
                  onClick={() => {
                    addToCart({
                      product,
                      startDate,
                      endDate,
                      time,
                      size: selectedSize || null,
                    });
                    handleBlink();
                    setOpenModal(false);
                  }}
                >
                  ยืนยันการจอง
                </button>
              </div>
            </div>
          </div>
        )}

        {showLoginModal && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-white rounded-xl w-full max-w-sm p-6">
              <h2 className="text-xl font-bold mb-2">ยังไม่ได้เข้าสู่ระบบ</h2>
              <p className="text-gray-600 mb-6">
                คุณต้องเข้าสู่ระบบก่อนจึงจะสามารถจองสินค้าได้
              </p>

              <div className="flex justify-end gap-2">
                <button
                  className="px-4 py-2 border rounded"
                  onClick={() => setShowLoginModal(false)}
                >
                  ยกเลิก
                </button>

                <button
                  className="px-4 py-2 bg-black text-white rounded"
                  onClick={() => {
                    setShowLoginModal(false);
                    navigate("/login");
                  }}
                >
                  เข้าสู่ระบบ
                </button>
              </div>
            </div>
          </div>
        )}

        {/* รูปสินค้า */}
        <div>
          <img
            src={image}
            alt={product.name}
            className="rounded-lg w-full h-[480px] object-contain bg-gray-50 p-5"
          />

          <div className="flex flex-wrap gap-3 mt-4">
            {product.images.map((img, index) => (
              <img
                key={index}
                src={img}
                alt=""
                className={`w-20 h-20 object-contain rounded-lg cursor-pointer border
        ${
          img === image
            ? "border-black"
            : "border-gray-200 hover:border-gray-400"
        }
      `}
                onClick={() => setImage(img)}
              />
            ))}
          </div>
        </div>

        {/* รายละเอียด */}
        <div>
          <h1 className="text-3xl font-bold">{product.name}</h1>
          <p className="text-gray-500 mt-1">{product.brand}</p>

          <div className="flex items-center gap-2 mt-2 text-sm text-gray-600">
            <Star size={16} className="fill-yellow-400 text-yellow-400" />
            <span>
              {product.rating} ({product.review_count} รีวิว)
            </span>
          </div>

          <p className="text-4xl text-green-600 font-bold mt-5">
            ฿{product.price.toLocaleString()}
            <span className="text-sm text-gray-500 font-normal"> / วัน</span>
          </p>

          <div className="mt-8 border-t pt-6">
            <h3 className="font-semibold text-lg mb-2">คำอธิบาย</h3>
            <p className="text-gray-700 leading-relaxed">
              {product.description}
            </p>
          </div>

          <div className="mt-8 bg-gray-50 rounded-lg p-4">
            <h3 className="font-semibold text-lg mb-3">ข้อมูลทางเทคนิค</h3>

            <ul className="space-y-2 text-sm">
              {product.specs?.capacity && (
                <li className="flex gap-2 items-center">
                  <Check size={16} /> ความจุ: {product.specs.capacity}
                </li>
              )}

              {product.specs?.weight_kg && (
                <li className="flex gap-2">
                  <Check size={16} /> น้ำหนัก: {product.specs.weight_kg}{" "}
                  กิโลกรัม
                </li>
              )}

              {product.specs?.material && (
                <li className="flex gap-2">
                  <Check size={16} /> วัสดุ: {product.specs.material}
                </li>
              )}

              {product.specs?.waterproof !== undefined && (
                <li className="flex gap-2">
                  {product.specs.waterproof ? (
                    <Check size={16} />
                  ) : (
                    <X size={16} />
                  )}{" "}
                  กันน้ำ:
                  {product.specs.waterproof ? " ได้" : " ไม่ได้"}
                </li>
              )}
              {product.specs?.brightness_lumen && (
                <li className="flex gap-2">
                  <Check size={16} /> ความสว่าง:{" "}
                  {product.specs.brightness_lumen} ลูเมน
                </li>
              )}

              {product.specs?.dimensions_cm && (
                <li className="flex gap-2">
                  <Check size={16} /> ขนาด (กxยxส):{" "}
                  {product.specs.dimensions_cm} ซม.
                </li>
              )}
              {product.specs?.adjustable && (
                <li className="flex gap-2">
                  <Check size={16} /> ปรับความยาวได้: {product.specs.adjustable}
                </li>
              )}
              {product.specs?.mode && (
                <li className="flex gap-2">
                  <Check size={16} /> {product.specs.mode}
                </li>
              )}
              {product.specs?.battery_type && (
                <li className="flex gap-2">
                  <Check size={16} /> ประเภทแบตเตอรี่:{" "}
                  {product.specs.battery_type}
                </li>
              )}
            </ul>
          </div>

          <div className="mt-6">
            <h3 className="font-semibold text-lg mb-2">เหมาะสำหรับ</h3>
            <div className="flex flex-wrap gap-2">
              {product.suitable_for.map((item, index) => (
                <span
                  key={index}
                  className="px-3 py-1 text-sm bg-gray-100 rounded-full"
                >
                  {item}
                </span>
              ))}
            </div>
          </div>

          {/* ระดับการใช้งาน */}
          <div className="mt-4">
            <h3 className="font-semibold text-lg mb-1">ระดับผู้ใช้งาน</h3>
            <span className="inline-block px-3 py-1 text-sm bg-green-100 text-green-700 rounded-full">
              {product.level}
            </span>
          </div>

          {product.category === "shoes" && (
            <div className="mt-6">
              <h3 className="font-semibold text-lg mb-2">ไซส์รองเท้า</h3>

              <div className="flex flex-wrap gap-2">
                {product.variants.map((v) => (
                  <button
                    key={v.size}
                    disabled={v.stock === 0}
                    onClick={() => setSelectedSize(v.size)}
                    className={`px-4 py-2 rounded border text-sm
            ${
              v.stock === 0
                ? "bg-gray-100 text-gray-400 cursor-not-allowed"
                : selectedSize === v.size
                  ? "bg-black text-white border-black"
                  : "bg-white hover:bg-gray-100"
            }
          `}
                  >
                    {v.size}
                  </button>
                ))}
              </div>

              {/* แสดงสถานะ */}
              {selectedSize && (
                <p className="text-sm text-green-600 mt-2">
                  เลือกไซส์: {selectedSize}
                </p>
              )}
            </div>
          )}

          {/* ปุ่ม */}
          <button
            className={`mt-6 w-full py-3 rounded text-white ${
              totalStock === 0 ||
              (product.category === "shoes" && !selectedSize)
                ? "bg-gray-400 cursor-not-allowed"
                : "bg-black hover:bg-gray-800"
            }`}
            disabled={
              totalStock === 0 ||
              (product.category === "shoes" && !selectedSize)
            }
            onClick={() => {
              if (!user) {
                setShowLoginModal(true);
                return;
              }

              setSelectedProduct(product);
              setOpenModal(true);
            }}
          >
            {product.category === "shoes"
              ? selectedSize
                ? "จองสินค้าเช่า"
                : "กรุณาเลือกไซส์"
              : totalStock === 0
                ? "สินค้าหมด"
                : "จองสินค้าเช่า"}
          </button>
          <div className="flex justify-end text-sm mt-2">
            <p>พร้อมให้เช่า: {totalStock} ชิ้น</p>
          </div>
        </div>
      </div>
    </div>
  );
}
