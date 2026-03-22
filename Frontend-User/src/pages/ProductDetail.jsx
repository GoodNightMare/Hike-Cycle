import { useParams } from "react-router-dom";
import { Star, Check, X, ArrowLeft } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCart } from "../context/CartContext";
import { useAuth } from "../context/AuthContext";
import axios from "axios";

export default function ProductDetail() {
  const navigate = useNavigate();
  const { addToCart } = useCart();
  const { id } = useParams();
  const { user } = useAuth();

  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);

  const afterTomorrow = new Date();
  afterTomorrow.setDate(afterTomorrow.getDate() + 2);

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
  const [image, setImage] = useState(null);
  const [selectedSize, setSelectedSize] = useState("");

  const safeParse = (str, defaultValue = null) => {
    try {
      return str ? JSON.parse(str) : defaultValue;
    } catch (e) {
      console.error("JSON Parse Error:", e);
      return defaultValue;
    }
  };

  // ถอดรหัส JSON String จาก API ให้เป็น Object
  const specs = safeParse(product?.specs);
  const suitableFor = safeParse(product?.suitableFor, []);
  const variants = safeParse(product?.variants, []);

  // แก้ไข totalStock สำหรับรองเท้าให้ดึงจาก variants ที่ parse แล้ว
  const totalStock =
    product?.category === "shoes" && variants
      ? variants.reduce((sum, v) => sum + v.stock, 0)
      : (product?.stock ?? 0);

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        setLoading(true);
        const response = await axios.get(
          `http://localhost:5279/api/products/${id}`,
        );
        setProduct(response.data);
        if (
          response.data.productImages &&
          response.data.productImages.length > 0
        ) {
          setImage(response.data.productImages[0].imageUrl);
        }
        setError(null);
      } catch (err) {
        setError("ไม่สามารถโหลดข้อมูลสินค้าได้");
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchProduct();
  }, [id]);

  const isValidTime = time >= "08:00" && time <= "20:00";

  const handleBlink = () => {
    setBlink(true);
    setTimeout(() => setBlink(false), 800);
  };

  if (loading) {
    return <p className="text-center mt-10">กำลังโหลด...</p>;
  }

  if (error) {
    return <p className="text-center mt-10 text-red-500">{error}</p>;
  }

  if (!product) {
    return <p className="text-center mt-10">ไม่พบสินค้า</p>;
  }

  return (
    <div className=" min-h-screen">
      <div className="max-w-6xl mx-auto p-2 bg-white rounded-xl shadow grid md:grid-cols-2 gap-8 relative">
        {blink && <div className="blink-screen" />}
        {openModal && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 w-full max-w-md">
              <h2 className="text-xl font-bold mb-4">
                จองสินค้า: {product?.name}
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
        <div className="relative">
          <button
            onClick={() => navigate(-1)}
            className="absolute top-3 left-3 z-10 px-2 py-1 bg-white rounded-full shadow hover:bg-gray-100 transition"
          >
            <ArrowLeft size={20} />
          </button>
          <img
            src={image}
            alt={product.name}
            className="rounded-lg w-full h-[480px] object-contain bg-gray-50 p-5"
          />

          <div className="flex flex-wrap gap-3 mt-4">
            {product.productImages?.map((img, index) => (
              <img
                key={index}
                src={img.imageUrl}
                alt=""
                className={`w-20 h-20 object-contain rounded-lg cursor-pointer border
        ${
          img.imageUrl === image
            ? "border-black"
            : "border-gray-200 hover:border-gray-400"
        }
      `}
                onClick={() => setImage(img.imageUrl)}
              />
            ))}
          </div>
        </div>

        {/* รายละเอียด */}
        <div>
          <h1 className="text-3xl font-bold">{product.name}</h1>
          <p className="text-gray-500 mt-1">{product.brand}</p>

          <div className="flex items-center gap-2 mt-2 text-sm text-gray-600 underline hover:text-red-500 cursor-pointer">
            <Star size={16} className="fill-yellow-400 text-yellow-400" />
            <span
              onClick={()=>navigate(`/products/${product.id}/reviews`)}
            >
              {product.rating} ({product.reviewCount} รีวิว)
            </span>
          </div>

          <p className="text-4xl text-green-600 font-bold mt-5">
            ฿{product.pricePerDay?.toLocaleString() ?? "N/A"}
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
              {specs?.capacity && (
                <li className="flex gap-2 items-center">
                  <Check size={16} /> ความจุ: {specs.capacity}
                </li>
              )}

              {specs?.weight_kg && (
                <li className="flex gap-2">
                  <Check size={16} /> น้ำหนัก: {specs.weight_kg} กิโลกรัม
                </li>
              )}

              {specs?.material && (
                <li className="flex gap-2">
                  <Check size={16} /> วัสดุ: {specs.material}
                </li>
              )}

              {specs?.waterproof !== undefined && (
                <li className="flex gap-2">
                  {specs.waterproof ? <Check size={16} /> : <X size={16} />}{" "}
                  กันน้ำ:
                  {product.specs.waterproof ? " ได้" : " ไม่ได้"}
                </li>
              )}
              {specs?.brightness_lumen && (
                <li className="flex gap-2">
                  <Check size={16} /> ความสว่าง: {specs.brightness_lumen} ลูเมน
                </li>
              )}

              {specs?.dimensions_cm && (
                <li className="flex gap-2">
                  <Check size={16} /> ขนาด (กxยxส): {specs.dimensions_cm} ซม.
                </li>
              )}
              {specs?.adjustable && (
                <li className="flex gap-2">
                  <Check size={16} /> ปรับความยาวได้: {specs.adjustable}
                </li>
              )}
              {specs?.mode && (
                <li className="flex gap-2">
                  <Check size={16} /> {specs.mode}
                </li>
              )}
              {specs?.battery_type && (
                <li className="flex gap-2">
                  <Check size={16} /> ประเภทแบตเตอรี่: {specs.battery_type}
                </li>
              )}
            </ul>
          </div>

          <div className="mt-6">
            <h3 className="font-semibold text-lg mb-2">เหมาะสำหรับ</h3>
            <div className="flex flex-wrap gap-2">
              {suitableFor?.map((item, index) => (
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
                {variants.map((v) => (
                  <button
                    key={v.size}
                    disabled={v.stock === 0}
                    onClick={() => setSelectedSize(v.size)}
                    className={`px-4 py-2 rounded border text-sm ${
                      v.stock === 0
                        ? "bg-gray-100 text-gray-400 cursor-not-allowed"
                        : selectedSize === v.size
                          ? "bg-black text-white border-black"
                          : "bg-white hover:bg-gray-100"
                    }`}
                  >
                    {v.size}({v.stock})
                  </button>
                ))}
              </div>
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
              setOpenModal(true);
            }}
          >
            {totalStock === 0 ? "สินค้าหมด" : "จองสินค้าเช่า"}
          </button>
          <div className="flex justify-end text-sm mt-2">
            <p>พร้อมให้เช่า: {totalStock} ชิ้น</p>
          </div>
        </div>
      </div>
    </div>
  );
}
