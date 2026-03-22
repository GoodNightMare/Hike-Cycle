import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Star, ChevronLeft, User } from 'lucide-react';
import axios from 'axios';

const Reviews = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // ดึงข้อมูลรีวิวจาก API
    const fetchReviews = async () => {
      try {
        const res = await axios.get(`http://localhost:5279/api/products/${id}/reviews`);
        setReviews(res.data);
      } catch (err) {
        console.error("Error fetching reviews:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchReviews();
  }, [id]);

  if (loading) return <div className="p-10 text-center">กำลังโหลดรีวิว...</div>;

  // คำนวณ Rating Breakdown
  const total = reviews.length;
  const stats = [5, 4, 3, 2, 1].map(star => ({
    star,
    count: reviews.filter(r => r.rating === star).length,
    percent: total > 0 ? (reviews.filter(r => r.rating === star).length / total) * 100 : 0
  }));

  return (
    <div className="max-w-3xl mx-auto p-6">
      {/* ปุ่มย้อนกลับ */}
      <button 
        onClick={() => navigate(-1)}
        className="flex items-center text-gray-500 hover:text-black mb-6 transition-colors"
      >
        <ChevronLeft size={20} />
        <span>ย้อนกลับไปหน้าสินค้า</span>
      </button>

      <h1 className="text-3xl font-bold mb-8">รีวิวจากผู้เช่าจริง</h1>

      {/* ส่วนสรุปคะแนน (Rating Summary Card) */}
      <div className="bg-white border rounded-3xl p-8 mb-10 shadow-sm flex flex-col md:flex-row gap-10 items-center">
        <div className="text-center">
          <p className="text-6xl font-black mb-2">
            {(reviews.reduce((s, r) => s + r.rating, 0) / (total || 1)).toFixed(1)}
          </p>
          <div className="flex justify-center mb-1">
            {[...Array(5)].map((_, i) => (
              <Star key={i} size={18} className="fill-yellow-400 text-yellow-400" />
            ))}
          </div>
          <p className="text-gray-400 text-sm">{total} รีวิว</p>
        </div>

        {/* กราฟแท่งคะแนน */}
        <div className="flex-1 w-full space-y-2">
          {stats.map((item) => (
            <div key={item.star} className="flex items-center gap-4">
              <span className="text-sm font-medium w-4">{item.star}</span>
              <div className="flex-1 h-2 bg-gray-100 rounded-full overflow-hidden">
                <div 
                  className="h-full bg-yellow-400 rounded-full" 
                  style={{ width: `${item.percent}%` }}
                />
              </div>
              <span className="text-sm text-gray-400 w-8">{item.count}</span>
            </div>
          ))}
        </div>
      </div>

      {/* รายการรีวิว */}
      <div className="space-y-8">
        {reviews.length > 0 ? (
          reviews.map((rev) => (
            <div key={rev.id} className="border-b border-gray-100 pb-8">
              <div className="flex items-center gap-4 mb-4">
                <div className="w-12 h-12 bg-gray-100 rounded-full flex items-center justify-center text-gray-400">
                  <User size={24} />
                </div>
                <div>
                  <h4 clas sName="font-bold text-gray-900">{rev.userName}</h4>
                  <div className="flex gap-1">
                    {[...Array(5)].map((_, i) => (
                      <Star 
                        key={i} 
                        size={14} 
                        className={i < rev.rating ? "fill-yellow-400 text-yellow-400" : "text-gray-200"} 
                      />
                    ))}
                  </div>
                </div>
                <span className="ml-auto text-sm text-gray-400">
                  {new Date(rev.createdAt).toLocaleDateString('th-TH', { year: 'numeric', month: 'long', day: 'numeric' })}
                </span>
              </div>
              <p className="text-gray-600 leading-relaxed pl-16">
                {rev.comment}
              </p>
            </div>
          ))
        ) : (
          <div className="text-center py-20 text-gray-400">
            ยังไม่มีรีวิวสำหรับสินค้านี้
          </div>
        )}
      </div>
    </div>
  );
};

export default Reviews;