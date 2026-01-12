import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

export default function PromoBanner({ products = [] }) {
  const navigate = useNavigate();
  const [currentSlide, setCurrentSlide] = useState(0);

  const benefits = [
    { icon: "🚚", title: "GIÁ SHIP ƯU ĐÃI", desc: "Với tất cả các đơn hàng" },
    { icon: "💰", title: "CAM KẾT GIÁ TỐT NHẤT", desc: "Đảm bảo chất lượng 100%" },
    { icon: "🕐", title: "HỖ TRỢ 24/7", desc: "Giải đáp mọi thắc mắc" },
    { icon: "🤝", title: "HỢP TÁC KINH DOANH", desc: "Bán sỉ, bán lẻ" },
  ];

  // Auto slide
  useEffect(() => {
    if (products.length === 0) return;
    const timer = setInterval(() => {
      setCurrentSlide((prev) => (prev + 1) % Math.ceil(products.length / 3));
    }, 5000);
    return () => clearInterval(timer);
  }, [products.length]);

  const promoProducts = products.slice(0, 9);
  const totalSlides = Math.ceil(promoProducts.length / 3);
  const visibleProducts = promoProducts.slice(currentSlide * 3, currentSlide * 3 + 3);

  const nextSlide = () => {
    setCurrentSlide((prev) => (prev + 1) % totalSlides);
  };

  const prevSlide = () => {
    setCurrentSlide((prev) => (prev - 1 + totalSlides) % totalSlides);
  };

  return (
    <div className="bg-white">
      {/* Benefits Bar */}
      <div className="border-b">
        <div className="max-w-7xl mx-auto px-6 py-4">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            {benefits.map((item, index) => (
              <div key={index} className="flex items-center gap-3">
                <div className="w-12 h-12 rounded-full bg-pink-100 flex items-center justify-center text-2xl">
                  {item.icon}
                </div>
                <div>
                  <p className="font-bold text-pink-600 text-sm">{item.title}</p>
                  <p className="text-gray-500 text-xs">{item.desc}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Promo Section */}
      <div className="max-w-7xl mx-auto px-6 py-8">
        <div className="flex gap-6">
          {/* Left: Promo Products Slider */}
          <div className="flex-1">
            <div className="flex items-stretch gap-4">
              {/* Promo Label */}
              <div className="w-16 h-64 bg-gradient-to-b from-pink-500 to-pink-600 rounded-lg flex items-center justify-center">
                <span className="text-white font-bold text-sm writing-vertical transform -rotate-180" style={{ writingMode: 'vertical-rl' }}>
                  SẢN PHẨM KHUYẾN MÃI
                </span>
              </div>

              {/* Slider */}
              <div className="flex-1 relative">
                {/* Prev Button */}
                <button
                  onClick={prevSlide}
                  className="absolute left-0 top-1/2 -translate-y-1/2 -translate-x-4 z-10 w-10 h-10 bg-white shadow-lg rounded-full flex items-center justify-center hover:bg-gray-50"
                >
                  <svg className="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                  </svg>
                </button>

                {/* Products */}
                <div className="grid grid-cols-3 gap-4">
                  {visibleProducts.map((product) => (
                    <div
                      key={product.id}
                      onClick={() => navigate(`/products/${product.id}`)}
                      className="bg-white border rounded-lg p-3 cursor-pointer hover:shadow-lg transition"
                    >
                      <div className="relative">
                        <img
                          src={product.imageUrl || "/OIP.jpg"}
                          alt={product.name}
                          className="w-full h-36 object-cover rounded"
                        />
                        {product.discount && (
                          <span className="absolute top-2 left-2 bg-pink-500 text-white text-xs px-2 py-1 rounded">
                            -{product.discount}%
                          </span>
                        )}
                      </div>
                      <h3 className="mt-2 text-sm font-medium text-gray-800 line-clamp-2">{product.name}</h3>
                      <div className="mt-1 flex items-center gap-2">
                        <span className="text-pink-500 font-bold">
                          {product.price?.toLocaleString("vi-VN")} đ
                        </span>
                        {product.originalPrice && (
                          <span className="text-gray-400 text-xs line-through">
                            {product.originalPrice?.toLocaleString("vi-VN")} đ
                          </span>
                        )}
                      </div>
                    </div>
                  ))}
                </div>

                {/* Next Button */}
                <button
                  onClick={nextSlide}
                  className="absolute right-0 top-1/2 -translate-y-1/2 translate-x-4 z-10 w-10 h-10 bg-white shadow-lg rounded-full flex items-center justify-center hover:bg-gray-50"
                >
                  <svg className="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                  </svg>
                </button>

                {/* Dots */}
                <div className="flex justify-center gap-2 mt-4">
                  {Array.from({ length: totalSlides }).map((_, index) => (
                    <button
                      key={index}
                      onClick={() => setCurrentSlide(index)}
                      className={`w-3 h-3 rounded-full transition ${
                        currentSlide === index ? "bg-pink-500" : "bg-gray-300"
                      }`}
                    />
                  ))}
                </div>
              </div>
            </div>
          </div>

          {/* Right: Best Seller Banner */}
          <div className="w-72 hidden lg:block">
            <div className="relative h-full rounded-lg overflow-hidden">
              <img
                src="/OIP.jpg"
                alt="Best Seller"
                className="w-full h-full object-cover"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent" />
              <div className="absolute bottom-4 right-4 text-right">
                <p className="text-white text-sm">SẢN PHẨM</p>
                <p className="text-pink-400 font-bold text-2xl">BÁN CHẠY</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
