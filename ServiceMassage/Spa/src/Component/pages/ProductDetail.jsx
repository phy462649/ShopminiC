import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { mockProducts } from "../../data/mockProducts";

export default function ProductDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [quantity, setQuantity] = useState(1);

  const product = mockProducts.find((p) => p.id === Number(id));
  const relatedProducts = mockProducts
    .filter((p) => p.categoryId === product?.categoryId && p.id !== product?.id)
    .slice(0, 4);

  if (!product) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-800 mb-4">Không tìm thấy sản phẩm</h2>
          <button
            onClick={() => navigate("/products")}
            className="px-6 py-2 bg-pink-500 text-white rounded-lg hover:bg-pink-600"
          >
            Quay lại danh sách
          </button>
        </div>
      </div>
    );
  }

  const handleAddToCart = () => {
    // TODO: Add to cart logic
    alert(`Đã thêm ${quantity} sản phẩm "${product.name}" vào giỏ hàng!`);
  };

  const handleBuyNow = () => {
    // TODO: Buy now logic
    navigate("/checkout", { state: { product, quantity } });
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-7xl mx-auto px-6">
        {/* Breadcrumb */}
        <nav className="mb-6 text-sm">
          <ol className="flex items-center gap-2 text-gray-500">
            <li><button onClick={() => navigate("/")} className="hover:text-pink-500">Trang chủ</button></li>
            <li>/</li>
            <li><button onClick={() => navigate("/products")} className="hover:text-pink-500">Sản phẩm</button></li>
            <li>/</li>
            <li className="text-gray-800">{product.name}</li>
          </ol>
        </nav>

        {/* Product Info */}
        <div className="bg-white rounded-lg shadow-md p-6 mb-8">
          <div className="grid md:grid-cols-2 gap-8">
            {/* Image */}
            <div className="relative">
              <img
                src={product.imageUrl || "/OIP.jpg"}
                alt={product.name}
                className="w-full h-96 object-cover rounded-lg"
              />
              {product.discount && (
                <span className="absolute top-4 left-4 bg-pink-500 text-white px-3 py-1 rounded-full text-sm font-semibold">
                  -{product.discount}%
                </span>
              )}
            </div>

            {/* Details */}
            <div>
              <span className="text-pink-500 text-sm font-medium">{product.categoryName}</span>
              <h1 className="text-2xl font-bold text-gray-800 mt-2 mb-4">{product.name}</h1>
              
              {/* Rating */}
              <div className="flex items-center gap-2 mb-4">
                <div className="flex text-yellow-400">
                  {[...Array(5)].map((_, i) => (
                    <svg key={i} className={`w-5 h-5 ${i < Math.floor(product.rating) ? "fill-current" : "fill-gray-300"}`} viewBox="0 0 20 20">
                      <path d="M10 15l-5.878 3.09 1.123-6.545L.489 6.91l6.572-.955L10 0l2.939 5.955 6.572.955-4.756 4.635 1.123 6.545z" />
                    </svg>
                  ))}
                </div>
                <span className="text-gray-600">({product.rating})</span>
                <span className="text-gray-400">|</span>
                <span className="text-gray-600">{product.reviews} đánh giá</span>
              </div>

              {/* Price */}
              <div className="flex items-center gap-4 mb-6">
                <span className="text-3xl font-bold text-pink-500">
                  {product.price?.toLocaleString("vi-VN")}đ
                </span>
                {product.originalPrice && (
                  <span className="text-xl text-gray-400 line-through">
                    {product.originalPrice?.toLocaleString("vi-VN")}đ
                  </span>
                )}
              </div>

              {/* Description */}
              <p className="text-gray-600 mb-6 leading-relaxed">{product.description}</p>

              {/* Stock */}
              <p className="text-sm text-gray-500 mb-6">
                Còn lại: <span className="font-semibold text-green-600">{product.stock} sản phẩm</span>
              </p>

              {/* Quantity */}
              <div className="flex items-center gap-4 mb-6">
                <span className="text-gray-700">Số lượng:</span>
                <div className="flex items-center border rounded-lg">
                  <button
                    onClick={() => setQuantity(Math.max(1, quantity - 1))}
                    className="px-4 py-2 hover:bg-gray-100"
                  >
                    -
                  </button>
                  <span className="px-4 py-2 border-x">{quantity}</span>
                  <button
                    onClick={() => setQuantity(Math.min(product.stock, quantity + 1))}
                    className="px-4 py-2 hover:bg-gray-100"
                  >
                    +
                  </button>
                </div>
              </div>

              {/* Buttons */}
              <div className="flex gap-4">
                <button
                  onClick={handleAddToCart}
                  className="flex-1 px-6 py-3 border-2 border-pink-500 text-pink-500 rounded-lg hover:bg-pink-50 font-semibold"
                >
                  Thêm vào giỏ
                </button>
                <button
                  onClick={handleBuyNow}
                  className="flex-1 px-6 py-3 bg-pink-500 text-white rounded-lg hover:bg-pink-600 font-semibold"
                >
                  Mua ngay
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Related Products */}
        {relatedProducts.length > 0 && (
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-bold text-gray-800 mb-6">Sản phẩm liên quan</h2>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              {relatedProducts.map((p) => (
                <div
                  key={p.id}
                  onClick={() => navigate(`/products/${p.id}`)}
                  className="bg-white border rounded-lg p-3 cursor-pointer hover:shadow-lg transition group"
                >
                  <div className="relative">
                    <img
                      src={p.imageUrl || "/OIP.jpg"}
                      alt={p.name}
                      className="w-full h-36 object-cover rounded group-hover:scale-105 transition"
                    />
                    {p.discount && (
                      <span className="absolute top-2 left-2 bg-pink-500 text-white text-xs px-2 py-1 rounded">
                        -{p.discount}%
                      </span>
                    )}
                  </div>
                  <h3 className="mt-2 text-sm font-medium text-gray-800 line-clamp-2">{p.name}</h3>
                  <div className="mt-1 flex items-center gap-2">
                    <span className="text-pink-500 font-bold text-sm">
                      {p.price?.toLocaleString("vi-VN")}đ
                    </span>
                    {p.originalPrice && (
                      <span className="text-gray-400 text-xs line-through">
                        {p.originalPrice?.toLocaleString("vi-VN")}đ
                      </span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
