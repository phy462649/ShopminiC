import { useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { productService, categoryService } from "../../services";
import { mockProducts, mockCategories } from "../../data/mockProducts";
// import ImageSlider from "../common/ImageSlider";
import PromoBanner from "../common/PromoBanner";
import CategoryProductSection from "../common/CategoryProductSection";
import SpaFeatures from "../common/SpaFeatures";

export default function Home() {
  const navigate = useNavigate();

  const { data: products = [] } = useQuery({
    queryKey: ["products"],
    queryFn: productService.getAll,
  });

  const { data: categories = [] } = useQuery({
    queryKey: ["categories"],
    queryFn: categoryService.getAll,
  });

  const productList = Array.isArray(products) && products.length > 0 ? products : mockProducts;
  const categoryList = Array.isArray(categories) && categories.length > 0 ? categories : mockCategories;

  // Fake promo products (thêm discount)
  const promoProducts = productList.slice(0, 9).map((p, i) => ({
    ...p,
    discount: [10, 20, 15, 25, 30, 10, 20, 15, 25][i] || 10,
    originalPrice: Math.round(p.price * 1.2),
  }));

  return (
    <div className="flex flex-col items-center">
      {/* Image Slider */}
      {/* <ImageSlider />  */}

      {/* Hero Section */}
      <section className="bg-gradient-to-r from-pink-100 to-purple-100 py-16 w-full">
        <div className="max-w-7xl mx-auto px-6">
          <div className="flex flex-col md:flex-row items-center justify-center gap-10">
            <div className="flex-1 text-center md:text-left">
              <h1 className="text-4xl md:text-5xl font-bold text-gray-800 mb-4">
                Chào mừng đến với <span className="text-pink-500">SPA Beauty</span>
              </h1>
              <p className="text-lg text-gray-600 mb-8">
                Nơi mang đến cho bạn sự thư giãn và làm đẹp hoàn hảo với các sản phẩm chất lượng cao
              </p>
              <div className="flex gap-4 justify-center md:justify-start">
                <button
                  onClick={() => navigate("/services")}
                  className="bg-pink-500 text-white px-8 py-3 rounded-full hover:bg-pink-600 transition"
                >
                  Khám phá dịch vụ
                </button>
                <button
                  onClick={() => navigate("/products")}
                  className="border-2 border-pink-500 text-pink-500 px-8 py-3 rounded-full hover:bg-pink-50 transition"
                >
                  Mua sản phẩm
                </button>
              </div>
            </div>
            <div className="flex-1 flex justify-center">
              <img src="/Banner.jpg" alt="Spa" className="rounded-2xl shadow-2xl w-full max-w-md" />
            </div>
          </div>
        </div>
      </section>

      {/* Promo Banner */}
      <div className="w-full">
        <PromoBanner products={promoProducts} />
      </div>

      {/* Category Product Section */}
      <div className="w-full">
        <CategoryProductSection categories={categoryList} products={productList} />
      </div>

      {/* Spa Features Section */}
      <div className="w-full">
        <SpaFeatures />
      </div>

      {/* Categories */}
      <section className="py-12 bg-gray-50 w-full">
        <div className="max-w-7xl mx-auto px-6">
          <h2 className="text-2xl font-bold text-center text-gray-800 mb-8">Danh mục sản phẩm</h2>
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4 justify-items-center">
            {categoryList.slice(0, 6).map((cat) => (
              <div
                key={cat.id}
                onClick={() => navigate(`/products?category=${cat.id}`)}
                className="bg-white rounded-lg p-4 text-center cursor-pointer hover:shadow-lg transition w-full max-w-[150px]"
              >
                <div className="w-16 h-16 bg-pink-100 rounded-full flex items-center justify-center mx-auto mb-3">
                  <span className="text-2xl">💆</span>
                </div>
                <p className="font-medium text-gray-800 text-sm">{cat.name}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Featured Products */}
      <section className="py-12 w-full">
        <div className="max-w-7xl mx-auto px-6">
          <div className="flex justify-between items-center mb-8">
            <h2 className="text-2xl font-bold text-gray-800">Sản phẩm nổi bật</h2>
            <button
              onClick={() => navigate("/products")}
              className="text-pink-500 hover:text-pink-600 font-medium"
            >
              Xem tất cả →
            </button>
          </div>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-6 justify-items-center">
            {productList.slice(0, 8).map((product) => (
              <div
                key={product.id}
                onClick={() => navigate(`/products/${product.id}`)}
                className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition group cursor-pointer w-full"
              >
                <div className="relative">
                  <img
                    src={product.imageUrl || "/OIP.jpg"}
                    alt={product.name}
                    className="w-full h-48 object-cover group-hover:scale-105 transition"
                  />
                  {product.discount && (
                    <span className="absolute top-2 left-2 bg-pink-500 text-white text-xs px-2 py-1 rounded">
                      -{product.discount}%
                    </span>
                  )}
                </div>
                <div className="p-4 text-center">
                  <h3 className="font-semibold text-gray-800 line-clamp-2">{product.name}</h3>
                  <div className="mt-2">
                    <p className="text-pink-500 font-bold">
                      {product.price?.toLocaleString("vi-VN")}đ
                    </p>
                    {product.originalPrice && (
                      <p className="text-gray-400 text-sm line-through">
                        {product.originalPrice?.toLocaleString("vi-VN")}đ
                      </p>
                    )}
                  </div>
                  <button
                    onClick={(e) => { e.stopPropagation(); navigate(`/products/${product.id}`); }}
                    className="mt-3 w-full bg-pink-500 text-white py-2 rounded hover:bg-pink-600"
                  >
                    Xem chi tiết
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Why Choose Us */}
      <section className="bg-pink-50 py-12 w-full">
        <div className="max-w-7xl mx-auto px-6">
          <h2 className="text-2xl font-bold text-center text-gray-800 mb-10">
            Tại sao chọn chúng tôi?
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6 justify-items-center">
            {[
              { icon: "💆", title: "Chuyên nghiệp", desc: "Đội ngũ được đào tạo bài bản" },
              { icon: "✨", title: "Chất lượng", desc: "Sản phẩm cao cấp, an toàn" },
              { icon: "💝", title: "Tận tâm", desc: "Phục vụ như người thân" },
              { icon: "🎁", title: "Ưu đãi", desc: "Nhiều khuyến mãi hấp dẫn" },
            ].map((item, index) => (
              <div key={index} className="bg-white rounded-lg p-6 text-center shadow-sm w-full max-w-[250px]">
                <div className="w-16 h-16 bg-pink-100 rounded-full flex items-center justify-center mx-auto mb-4">
                  <span className="text-3xl">{item.icon}</span>
                </div>
                <h3 className="font-semibold text-lg mb-2">{item.title}</h3>
                <p className="text-gray-600 text-sm">{item.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Photo Gallery */}
      <section className="py-12 w-full bg-white">
        <div className="max-w-7xl mx-auto px-6">
          <div className="flex justify-between items-center mb-8">
            <h2 className="text-3xl font-serif text-gray-800">Hình ảnh khách hàng :</h2>
            <button 
              onClick={() => navigate("/gallery")}
              className="text-pink-500 hover:text-pink-600 font-medium flex items-center gap-1"
            >
              Xem tất cả <span>→</span>
            </button>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="overflow-hidden rounded-lg">
              <img 
                src="/1.png" 
                alt="Spa massage" 
                className="w-full h-64 object-cover hover:scale-105 transition duration-300"
              />
            </div>
            <div className="overflow-hidden rounded-lg">
              <img 
                src="/customer2.png" 
                alt="Spa treatment" 
                className="w-full h-64 object-cover hover:scale-105 transition duration-300"
              />
            </div>
            <div className="overflow-hidden rounded-lg">
              <img 
                src="/customer1.png" 
                alt="Spa resort" 
                className="w-full h-64 object-cover hover:scale-105 transition duration-300"
              />
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
