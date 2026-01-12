import { useState } from "react";
import { useNavigate } from "react-router-dom";
import ServiceBanner from "../common/ServiceBanner";

export default function Services() {
  const navigate = useNavigate();
  const [selectedCategory, setSelectedCategory] = useState("all");

  const categories = [
    { id: "all", name: "Tất cả" },
    { id: "massage", name: "Massage" },
    { id: "facial", name: "Chăm sóc da" },
    { id: "body", name: "Chăm sóc body" },
    { id: "hair", name: "Tóc & Gội đầu" },
  ];

  const services = [
    { id: 1, name: "Massage thư giãn", category: "massage", price: 300000, duration: "60 phút", image: "/OIP.jpg" },
    { id: 2, name: "Massage đá nóng", category: "massage", price: 450000, duration: "90 phút", image: "/OIP.jpg" },
    { id: 3, name: "Chăm sóc da mặt cơ bản", category: "facial", price: 500000, duration: "60 phút", image: "/OIP.jpg" },
    { id: 4, name: "Trị mụn chuyên sâu", category: "facial", price: 700000, duration: "90 phút", image: "/OIP.jpg" },
    { id: 5, name: "Tắm trắng", category: "body", price: 800000, duration: "120 phút", image: "/OIP.jpg" },
    { id: 6, name: "Tẩy tế bào chết body", category: "body", price: 400000, duration: "60 phút", image: "/OIP.jpg" },
    { id: 7, name: "Gội đầu dưỡng sinh", category: "hair", price: 200000, duration: "45 phút", image: "/OIP.jpg" },
    { id: 8, name: "Ủ tóc phục hồi", category: "hair", price: 350000, duration: "60 phút", image: "/OIP.jpg" },
  ];

  const filteredServices = selectedCategory === "all" 
    ? services 
    : services.filter(s => s.category === selectedCategory);

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Service Banner */}
      <ServiceBanner />

      <div className="py-10">
        <div className="max-w-7xl mx-auto px-6">
          <h1 className="text-3xl font-bold text-center text-gray-800 mb-8">Dịch vụ của chúng tôi</h1>

        {/* Category Filter */}
        <div className="flex flex-wrap justify-center gap-3 mb-10">
          {categories.map((cat) => (
            <button
              key={cat.id}
              onClick={() => setSelectedCategory(cat.id)}
              className={`px-5 py-2 rounded-full transition ${
                selectedCategory === cat.id
                  ? "bg-pink-500 text-white"
                  : "bg-white text-gray-700 hover:bg-pink-100"
              }`}
            >
              {cat.name}
            </button>
          ))}
        </div>

        {/* Services Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {filteredServices.map((service) => (
            <div
              key={service.id}
              className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition"
            >
              <img
                src={service.image}
                alt={service.name}
                className="w-full h-48 object-cover"
              />
              <div className="p-4">
                <h3 className="font-semibold text-gray-800 mb-2">{service.name}</h3>
                <p className="text-gray-500 text-sm mb-2">⏱ {service.duration}</p>
                <p className="text-pink-500 font-bold text-lg mb-3">
                  {service.price.toLocaleString("vi-VN")}đ
                </p>
                <button
                  onClick={() => navigate("/booking", { state: { service } })}
                  className="w-full bg-pink-500 text-white py-2 rounded hover:bg-pink-600"
                >
                  Đặt lịch ngay
                </button>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
    </div>
  );
}
