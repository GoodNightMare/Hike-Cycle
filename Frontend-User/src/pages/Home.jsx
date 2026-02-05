// src/pages/Home.jsx
import { Flame } from "lucide-react";
import { useEffect, useState } from "react";
import promotionsData from "./../data/promotions.json";
import routesRecommended from "./../data/routesRecommended.json";

export default function Home() {
  const [promotions, setPromotions] = useState([]);
  const [banners, setBanners] = useState([]);

  const [current, setCurrent] = useState(0);

  useEffect(() => {
    setPromotions(promotionsData);
    
  }, []);

  useEffect(() => {
    setBanners(
      promotions.map((promotion) => {
        return `${promotion.title} : ${promotion.description}`;
      })
    );
  }, [promotions]);

  useEffect(() => {
    if (banners.length === 0) return;

    const timer = setInterval(() => {
      setCurrent((prev) => (prev + 1) % banners.length);
    }, 5000);

    return () => clearInterval(timer);
  }, [banners.length]);

  return (
    <div className="min-h-screen">
      {/* Banner */}
      <div className="relative h-96 mx-4 mt-4 rounded-xl overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-r from-[#0B6623] to-green-500 opacity-90" />

        <div className="relative z-10 h-full flex flex-col items-center justify-center text-center px-6">
          <h1 className="text-3xl md:text-4xl font-bold text-white max-w-3xl">
            {banners[current]}
          </h1>

          <p className="mt-4 text-white/80">
            เตรียมอุปกรณ์ให้พร้อม แล้วออกไปผจญภัยกับธรรมชาติ
          </p>
        </div>

        {/* Indicator */}
        <div className="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2 z-20">
          {banners.map((_, index) => (
            <span
              key={index}
              onClick={() => setCurrent(index)}
              className={`w-3 h-3 rounded-full cursor-pointer transition ${
                current === index ? "bg-white" : "bg-white/40"
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
          {routesRecommended.map((route, index) => (
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

              <button className="mt-4 text-m text-green-700 font-semibold hover:underline">
                ดูรายละเอียด →
              </button>
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
