import { useState } from "react";
import { useNavigate } from "react-router-dom";

export default function CategoryProductSection({ categories = [], products = [] }) {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState(0);
  const [activeSubCategory, setActiveSubCategory] = useState(null);

  // Main tabs (top categories)
  const mainTabs = [
    { id: "natural", name: "Mỹ phẩm thiên nhiên", icon: "*" },
    ...categories.slice(0, 3).map((cat) => ({ id: cat.id, name: cat.name })),
  ];

  // Sub categories for left sidebar
  const subCategories = [
    "Mỹ phẩm chăm sóc da mặt",
    "Mỹ phẩm chăm sóc tóc",
    "Sản phẩm Amenities",
  ];

  // Filter products based on active tab
  const getFilteredProducts = () => {
    if (activeTab === 0) {
      return products.slice(0, 6);
    }
    const categoryId = mainTabs[activeTab]?.id;
    return products.filter((p) => p.categoryId === categoryId).slice(0, 6);
  };

  const filteredProducts = getFilteredProducts();

  return (
    <section className="py-8 bg-white">
      <div className="max-w-6xl mx-auto px-6">
        {/* Tabs Header */}
        <div className="flex border-b">
          {mainTabs.map((tab, index) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(index)}
              className={`px-6 py-3 font-medium text-sm transition-colors relative ${
                activeTab === index
                  ? index === 0
                    ? "bg-pink-500 text-white"
                    : "text-pink-500 border-b-2 border-pink-500"
                  : "text-gray-600 hover:text-pink-500"
              }`}
            >
              {tab.icon && <span className="mr-2">{tab.icon}</span>}
              {tab.name}
            </button>
          ))}
        </div>

        {/* Content */}
        <div className="flex gap-6 mt-6">
          {/* Left Sidebar - Sub Categories */}
          <div className="w-48 hidden md:block">
            <ul className="space-y-1">
              {subCategories.map((sub, index) => (
                <li key={index}>
                  <button
                    onClick={() => setActiveSubCategory(index)}
                    className={`w-full text-left px-3 py-2 text-sm rounded transition ${
                      activeSubCategory === index
                        ? "bg-pink-50 text-pink-500"
                        : "text-gray-600 hover:bg-gray-50"
                    }`}
                  >
                    {sub}
                  </button>
                </li>
              ))}
            </ul>

            {/* Banner */}
            <div className="mt-4 relative rounded-lg overflow-hidden">
              <img
                src="/OIP.jpg"
                alt="Banner"
                className="w-full h-48 object-cover"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/50 to-transparent" />
              <button
                onClick={() => navigate("/products")}
                className="absolute bottom-4 left-1/2 -translate-x-1/2 bg-pink-500 text-white px-4 py-2 rounded text-sm hover:bg-pink-600"
              >
                Xem thêm
              </button>
            </div>
          </div>

          {/* Right - Products Grid */}
          <div className="flex-1">
            <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
              {filteredProducts.map((product) => (
                <div
                  key={product.id}
                  onClick={() => navigate("/products")}
                  className="bg-white border rounded-lg p-3 cursor-pointer hover:shadow-lg transition group"
                >
                  <div className="relative">
                    <img
                      src={product.imageUrl || "/OIP.jpg"}
                      alt={product.name}
                      className="w-full h-32 object-cover rounded group-hover:scale-105 transition"
                    />
                    {product.isNew && (
                      <span className="absolute top-2 left-2 bg-pink-500 text-white text-xs px-2 py-0.5 rounded">
                        Mới
                      </span>
                    )}
                    {product.discount && (
                      <span className="absolute top-2 right-2 bg-pink-500 text-white text-xs px-2 py-0.5 rounded">
                        -{product.discount}%
                      </span>
                    )}
                  </div>
                  <h3 className="mt-2 text-sm font-medium text-gray-800 line-clamp-2 min-h-[40px]">
                    {product.name}
                  </h3>
                  <p className="mt-1 text-sm">
                    <span className="text-gray-500">Giá bán: </span>
                    <span className="text-pink-500 font-bold">
                      {product.price?.toLocaleString("vi-VN")} đ
                    </span>
                  </p>
                </div>
              ))}
            </div>

            {/* View More Button */}
            <div className="text-center mt-6">
              <button
                onClick={() => navigate("/products")}
                className="px-8 py-2 border-2 border-pink-500 text-pink-500 rounded-full hover:bg-pink-500 hover:text-white transition"
              >
                Xem tất cả sản phẩm
              </button>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
