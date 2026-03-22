// src/pages/Products.jsx
import { useEffect, useState } from "react";
// import productsData from "../data/products.json";
import { Star,Search, SlidersHorizontal, Tag } from "lucide-react";
import { Link } from "react-router-dom";
import axios from "axios";

export default function Products() {
  const [search, setSearch] = useState("");
  const [category, setCategory] = useState("all");
  const [maxPrice, setMaxPrice] = useState("");
  const [productsData, setProductsData] = useState([]);

  useEffect(() => {
    axios.get("http://localhost:5279/api/products")
      .then((response) => {
        setProductsData(response.data);
      })
      .catch((error) => {
        console.error("Error fetching products:", error);
      });
  }, []);

  const CATEGORY_LABEL = {
    all: "ทุกประเภท",
    trekking_pole: "ไม้เท้าเดินป่า",
    backpack: "เป้",
    tent: "เต็นท์",
    shoes: "รองเท้า",
    cooking: "อุปกรณ์ทำอาหาร",
    lighting: "ไฟ / ไฟฉาย",
  };

  const products = productsData.filter((item) => {
    const matchName = item.name.toLowerCase().includes(search.toLowerCase());

    const matchCategory = category === "all" || item.category === category;

    const matchPrice = maxPrice === "" || item.price <= Number(maxPrice);

    return matchName && matchCategory && matchPrice;
  });

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-3xl font-bold">สินค้าอุปกรณ์เดินป่า</h1>
        <p className="text-gray-500 mt-1">
          เลือกอุปกรณ์คุณภาพ พร้อมออกเดินทางอย่างมั่นใจ
        </p>
      </div>

<div className="bg-white rounded-2xl p-5 shadow-sm mb-8">
  <div className="flex flex-wrap gap-4 items-center">

    {/* Search */}
    <div className="relative w-full sm:w-72">
      <Search
        size={18}
        className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"
      />
      <input
        type="text"
        placeholder="ค้นหาสินค้า..."
        className="w-full border pl-10 pr-3 py-2 rounded-lg
          focus:outline-none focus:ring-2 focus:ring-green-500"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />
    </div>

    {/* Category */}
    <div className="flex items-center gap-2">
      <SlidersHorizontal size={18} className="text-gray-500" />
      <select
        className="border px-3 py-2 rounded-lg
          focus:outline-none focus:ring-2 focus:ring-green-500"
        value={category}
        onChange={(e) => setCategory(e.target.value)}
      >
        {Object.entries(CATEGORY_LABEL).map(([key, label]) => (
          <option key={key} value={key}>
            {label}
          </option>
        ))}
      </select>
    </div>

    {/* Max Price */}
    <div className="relative w-full sm:w-40">
      <Tag
        size={18}
        className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"
      />
      <input
        type="number"
        placeholder="ราคาสูงสุด"
        className="w-full border pl-10 pr-3 py-2 rounded-lg
          focus:outline-none focus:ring-2 focus:ring-green-500"
        value={maxPrice}
        onChange={(e) => setMaxPrice(e.target.value)}
      />
    </div>

  </div>
</div>


      {/* 🛍️ Product List */}
      <div className="">
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-8">
          {products.length === 0 && (
            <p className="text-gray-500 col-span-full">ไม่พบสินค้า</p>
          )}

          {products.map((item) => {
let variants = [];
          try {
            variants = item.variants ? JSON.parse(item.variants) : [];
          } catch (e) {
            variants = [];
          }

          const totalStock = item.category === "shoes" && variants.length > 0
            ? variants.reduce((sum, v) => sum + v.stock, 0)
            : (item.stock || 0);
            return (
              <Link
                to={`/products/${item.id}`}
                key={item.id}
                className="bg-white rounded-xl p-4 hover:shadow-lg transition flex flex-col h-full"
              >
                {/* Image */}
                <div className="relative">
                  <img
                    src={item.productImages && item.productImages.length > 0 ? item.productImages[0].imageUrl : '/placeholder.jpg'}
                    alt={item.name}
                    className="h-72 m-auto object-cover rounded-lg"
                  />

                  {totalStock === 0 && (
                    <span className="absolute top-2 right-2 bg-red-500 text-white text-xs px-2 py-1 rounded">
                      สินค้าหมด
                    </span>
                  )}
                </div>

                {/* Info */}
                <div className="mt-3 flex-1">
                  <h2 className="font-semibold line-clamp-2 text-lg">
                    {item.name}
                  </h2>

                  <p className="text-sm text-gray-500">{item.brand}</p>

                  <div className="flex items-center gap-1 mt-1 text-sm">
                    <Star
                      size={16}
                      className="text-yellow-400 fill-yellow-400"
                    />
                    <span>{item.rating}</span>
                    <span className="text-gray-400">({item.review_count})</span>
                  </div>
                </div>

                {/* Footer */}
                <div className="mt-4">
                  <p className="text-green-600 font-bold text-xl">
                    ฿{item.pricePerDay.toLocaleString()}
                  </p>

                  <p
                    className={`text-sm mt-1 ${
                      totalStock === 0 ? "text-red-500" : "text-gray-600"
                    }`}
                  >
                    {totalStock === 0
                      ? "สินค้าหมด"
                      : `พร้อมให้เช่า ${totalStock} ชิ้น`}
                  </p>

                  <button className="mt-3 w-full py-2 rounded-lg text-white bg-black hover:bg-gray-800 transition">
                    ดูรายละเอียด
                  </button>
                </div>
              </Link>
            );
          })}
        </div>
      </div>
    </div>
  );
}
