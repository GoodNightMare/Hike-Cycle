// src/pages/Home.jsx
import { Flame } from "lucide-react";
import { useEffect, useState } from "react";
import axios from "axios";

export default function Home() {
  const [banners, setBanners] = useState([]);
  const [routes, setRoutes] = useState([]);
  const [loading, setLoading] = useState(true);

  const [current, setCurrent] = useState(0);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // ✅ ดึงข้อมูลพร้อมกันทั้ง 2 API
        const [promoRes, routeRes] = await Promise.all([
          axios.get("http://localhost:5279/api/promotions"),
          axios.get("http://localhost:5279/api/RecommendedRoutes"),
        ]);

        // 1. จัดการข้อมูล Banner
        if (promoRes.data.length > 0) {
          const formattedBanners = promoRes.data.map(
            (p) => `${p.title} : ${p.description}`,
          );
          setBanners(formattedBanners);
        } else {
          setBanners(["Hike-Cycle : อุปกรณ์เดินป่าคุณภาพดี"]);
        }

        // 2. จัดการข้อมูล Routes
        setRoutes(routeRes.data);

        setLoading(false);
      } catch (error) {
        console.error("Error fetching data:", error);
        setBanners(["ขออภัย ไม่สามารถดึงข้อมูลได้"]);
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    if (banners.length <= 1) return;

    const timer = setInterval(() => {
      setCurrent((prev) => (prev + 1) % banners.length);
    }, 5000);

    return () => clearInterval(timer);
  }, [banners.length]);

  return (
    <div className="min-h-screen">
      {/* Banner */}
      <div className="relative h-96 mx-4 mt-4 rounded-xl overflow-hidden shadow-lg">
        <div className="absolute inset-0 bg-gradient-to-r from-[#0B6623] to-green-600 opacity-90 transition-all" />

        <div className="relative z-10 h-full flex flex-col items-center justify-center text-center px-6">
          {loading ? (
            <div className="animate-pulse text-white">กำลังโหลดโปรโมชัน...</div>
          ) : (
            <>
              <h1 className="text-3xl md:text-5xl font-bold text-white max-w-4xl leading-tight transition-all duration-500">
                {banners[current]}
              </h1>
              <p className="mt-4 text-white/90 text-lg">
                เตรียมอุปกรณ์ให้พร้อม แล้วออกไปผจญภัยกับธรรมชาติ
              </p>
            </>
          )}
        </div>

        {/* Indicator */}
        <div className="absolute bottom-6 left-1/2 -translate-x-1/2 flex gap-3 z-20">
          {banners.map((_, index) => (
            <span
              key={index}
              onClick={() => setCurrent(index)}
              className={`w-3 h-3 rounded-full cursor-pointer transition-all duration-300 ${
                current === index
                  ? "bg-white scale-125"
                  : "bg-white/40 hover:bg-white/60"
              }`}
            />
          ))}
        </div>
      </div>

      {/* Route Recommend */}
      <div className="mt-10 px-4">
        <h2 className="text-2xl font-bold mb-6 flex items-center gap-2">
          เส้นทางแนะนำ
          <Flame className="text-red-500" fill="red" stroke="none" />
        </h2>

        <div className="grid md:grid-cols-2 gap-6">
          {routes.map((route, index) => (
            <div
              key={index}
              className="bg-white rounded-xl p-5 shadow-sm hover:shadow-lg transition"
            >
              <div className="flex justify-between items-start">
                <h3 className="text-lg font-semibold">{route.name}</h3>

                <span className="text-m px-2 py-1 rounded bg-green-100 text-green-700">
                  {route.level}
                </span>
              </div>

              <div className="mt-3 text-m text-gray-600 space-y-1">
                <p>📍 จังหวัด : {route.province}</p>
                <p>🗺️ ระยะทาง : {route.distance}</p>
                <p>⏱ ระยะเวลา : {route.duration}</p>
              </div>

              <p className="mt-3 text-m">
                🌄 <span className="font-medium">ไฮไลต์:</span>{" "}
                {route.highlight}
              </p>

              <p className="mt-2 text-m">
                🎒 <span className="font-medium">เหมาะกับ:</span>{" "}
                {route.suitable}
              </p>

              {/* <button className="mt-4 text-m text-green-700 font-semibold hover:underline">
                ดูรายละเอียด →
              </button> */}
            </div>
          ))}
        </div>
      </div>

      {/* expert */}
      <div className="mt-14 px-4">
        <h2 className="text-xl font-bold mb-3">ปรึกษาผู้เชี่ยวชาญ</h2>

        <div className="bg-gray-100 rounded-xl p-6 text-gray-600">
          กำลังพัฒนาฟีเจอร์สำหรับการให้คำแนะนำโดยผู้เชี่ยวชาญด้านการเดินป่า
        </div>
      </div>
    </div>
  );
}
