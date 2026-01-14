import { useState, useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { message } from "antd";
import { productService, categoryService } from "../../services";
import { mockProducts, mockCategories } from "../../data/mockProducts";

export default function Products() {
  const navigate = useNavigate();
  const [cart, setCart] = useState(() => {
    const saved = localStorage.getItem("cart");
    return saved ? JSON.parse(saved) : [];
  });
  const [selectedCategory, setSelectedCategory] = useState("all");
  const [searchTerm, setSearchTerm] = useState("");
  const [showSearchResults, setShowSearchResults] = useState(false);
  const [showCart, setShowCart] = useState(false);
  const searchRef = useRef(null);

  // Fetch products
  const { data: products = [], isLoading } = useQuery({
    queryKey: ["products"],
    queryFn: productService.getAll,
  });

  // Fetch categories
  const { data: categories = [] } = useQuery({
    queryKey: ["categories"],
    queryFn: categoryService.getAll,
  });

  // Save cart to localStorage
  useEffect(() => {
    localStorage.setItem("cart", JSON.stringify(cart));
  }, [cart]);

  // Close search results when clicking outside
  useEffect(() => {
    const handleClickOutside = (e) => {
      if (searchRef.current && !searchRef.current.contains(e.target)) {
        setShowSearchResults(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const productList = Array.isArray(products) && products.length > 0 ? products : mockProducts;
  const categoryList = Array.isArray(categories) && categories.length > 0 ? categories : mockCategories;

  // Search results (dropdown)
  const searchResults = searchTerm.length > 0
    ? productList.filter((p) => p.name?.toLowerCase().includes(searchTerm.toLowerCase())).slice(0, 5)
    : [];

  // Filter products for main grid
  const filteredProducts = productList.filter((product) => {
    const matchCategory = selectedCategory === "all" || product.categoryId === parseInt(selectedCategory);
    const matchSearch = searchTerm === "" || product.name?.toLowerCase().includes(searchTerm.toLowerCase());
    return matchCategory && matchSearch;
  });

  const addToCart = (product) => {
    const existing = cart.find((item) => item.id === product.id);
    if (existing) {
      setCart(cart.map((item) =>
        item.id === product.id ? { ...item, quantity: item.quantity + 1 } : item
      ));
    } else {
      setCart([...cart, { ...product, quantity: 1 }]);
    }
    message.success("Đã thêm vào giỏ hàng!");
  };

  const updateQuantity = (id, quantity) => {
    if (quantity <= 0) {
      setCart(cart.filter((item) => item.id !== id));
    } else {
      setCart(cart.map((item) => (item.id === id ? { ...item, quantity } : item)));
    }
  };

  const removeFromCart = (id) => {
    setCart(cart.filter((item) => item.id !== id));
  };

  const clearCart = () => {
    setCart([]);
    message.success("Đã xóa giỏ hàng!");
  };

  const handleSelectProduct = (product) => {
    setSearchTerm(product.name);
    setShowSearchResults(false);
  };

  const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
  const totalPrice = cart.reduce((sum, item) => sum + item.price * item.quantity, 0);

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-pink-500"></div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-10">
      <div className="max-w-7xl mx-auto px-6">
        <h1 className="text-3xl font-bold text-center text-gray-800 mb-8">Sản phẩm</h1>

        {/* Search & Filter */}
        <div className="flex flex-col md:flex-row gap-4 mb-8">
          {/* Search with dropdown */}
          <div className="relative flex-1" ref={searchRef}>
            <input
              type="text"
              placeholder="Tìm kiếm sản phẩm..."
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                setShowSearchResults(true);
              }}
              onFocus={() => setShowSearchResults(true)}
              className="w-full pl-10 pr-4 py-3 border rounded-lg focus:outline-none focus:ring-2 focus:ring-pink-500"
            />
            <svg className="w-5 h-5 absolute left-3 top-3.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            {searchTerm && (
              <button
                onClick={() => { setSearchTerm(""); setShowSearchResults(false); }}
                className="absolute right-3 top-3.5 text-gray-400 hover:text-gray-600"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            )}

            {/* Search Results Dropdown */}
            {showSearchResults && searchResults.length > 0 && (
              <div className="absolute top-full left-0 right-0 mt-1 bg-white border rounded-lg shadow-lg z-20 max-h-80 overflow-y-auto">
                {searchResults.map((product) => (
                  <div
                    key={product.id}
                    onClick={() => handleSelectProduct(product)}
                    className="flex items-center gap-3 p-3 hover:bg-gray-50 cursor-pointer border-b last:border-b-0"
                  >
                    <img
                      src={product.imageUrl || "/OIP.jpg"}
                      alt={product.name}
                      className="w-12 h-12 object-cover rounded"
                    />
                    <div className="flex-1 min-w-0">
                      <p className="font-medium text-gray-800 truncate">{product.name}</p>
                      <p className="text-pink-500 text-sm font-bold">
                        {product.price?.toLocaleString("vi-VN")}đ
                      </p>
                    </div>
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        addToCart(product);
                      }}
                      disabled={product.stock <= 0}
                      className="px-3 py-1 bg-pink-500 text-white text-sm rounded hover:bg-pink-600 disabled:bg-gray-300"
                    >
                      Thêm
                    </button>
                  </div>
                ))}
              </div>
            )}

            {/* No results */}
            {showSearchResults && searchTerm && searchResults.length === 0 && (
              <div className="absolute top-full left-0 right-0 mt-1 bg-white border rounded-lg shadow-lg z-20 p-4 text-center text-gray-500">
                Không tìm thấy sản phẩm "{searchTerm}"
              </div>
            )}
          </div>

          {/* Category Filter */}
          <select
            value={selectedCategory}
            onChange={(e) => setSelectedCategory(e.target.value)}
            className="px-4 py-3 border rounded-lg focus:outline-none focus:ring-2 focus:ring-pink-500 bg-white"
          >
            <option value="all">Tất cả danh mục</option>
            {categoryList.map((cat) => (
              <option key={cat.id} value={cat.id}>{cat.name}</option>
            ))}
          </select>

          {/* Cart Button */}
          <button
            onClick={() => setShowCart(true)}
            className="relative px-6 py-3 bg-pink-500 text-white rounded-lg hover:bg-pink-600 flex items-center gap-2"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
            Giỏ hàng
            {totalItems > 0 && (
              <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs w-6 h-6 rounded-full flex items-center justify-center">
                {totalItems}
              </span>
            )}
          </button>
        </div>

        {/* Results count */}
        {searchTerm && (
          <p className="mb-4 text-gray-600">
            Tìm thấy <span className="font-bold text-pink-500">{filteredProducts.length}</span> sản phẩm cho "{searchTerm}"
          </p>
        )}

        {/* Products Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {filteredProducts.map((product) => (
            <div
              key={product.id}
              className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition group cursor-pointer"
              onClick={() => navigate(`/products/${product.id}`)}
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
                {product.stock <= 0 && (
                  <div className="absolute inset-0 bg-black/50 flex items-center justify-center">
                    <span className="text-white font-bold">Hết hàng</span>
                  </div>
                )}
              </div>
              <div className="p-4">
                <span className="text-xs text-pink-500 bg-pink-50 px-2 py-1 rounded">
                  {product.categoryName || categoryList.find((c) => c.id === product.categoryId)?.name || "Khác"}
                </span>
                <h3 className="font-semibold text-gray-800 mt-2 mb-1 line-clamp-2">{product.name}</h3>
                <p className="text-gray-500 text-sm mb-2 line-clamp-2">{product.description}</p>
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-pink-500 font-bold text-lg">
                      {product.price?.toLocaleString("vi-VN")}đ
                    </p>
                    {product.originalPrice && (
                      <p className="text-gray-400 text-sm line-through">
                        {product.originalPrice?.toLocaleString("vi-VN")}đ
                      </p>
                    )}
                  </div>
                  <span className="text-xs text-gray-400">Còn {product.stock || 0}</span>
                </div>
                <button
                  onClick={(e) => { e.stopPropagation(); addToCart(product); }}
                  disabled={product.stock <= 0}
                  className="w-full mt-3 bg-pink-500 text-white py-2 rounded hover:bg-pink-600 disabled:bg-gray-300 disabled:cursor-not-allowed"
                >
                  {product.stock <= 0 ? "Hết hàng" : "Thêm vào giỏ"}
                </button>
              </div>
            </div>
          ))}
        </div>

        {filteredProducts.length === 0 && (
          <div className="text-center py-10 text-gray-500">
            Không tìm thấy sản phẩm nào
          </div>
        )}
      </div>

      {/* Cart Sidebar */}
      {showCart && (
        <div className="fixed inset-0 z-50">
          <div className="absolute inset-0 bg-black/50" onClick={() => setShowCart(false)} />
          <div className="absolute right-0 top-0 h-full w-full max-w-md bg-white shadow-xl">
            <div className="flex flex-col h-full">
              {/* Header */}
              <div className="flex items-center justify-between p-4 border-b">
                <h2 className="text-xl font-bold">Giỏ hàng ({totalItems})</h2>
                <button onClick={() => setShowCart(false)} className="p-2 hover:bg-gray-100 rounded">
                  <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>

              {/* Cart Items */}
              <div className="flex-1 overflow-y-auto p-4">
                {cart.length === 0 ? (
                  <div className="text-center py-10 text-gray-500">
                    Giỏ hàng trống
                  </div>
                ) : (
                  <div className="space-y-4">
                    {cart.map((item) => (
                      <div key={item.id} className="flex gap-4 bg-gray-50 p-3 rounded-lg">
                        <img
                          src={item.imageUrl || "/OIP.jpg"}
                          alt={item.name}
                          className="w-20 h-20 object-cover rounded"
                        />
                        <div className="flex-1">
                          <h3 className="font-medium text-sm line-clamp-2">{item.name}</h3>
                          <p className="text-pink-500 font-bold">{item.price?.toLocaleString("vi-VN")}đ</p>
                          <div className="flex items-center gap-2 mt-2">
                            <button
                              onClick={() => updateQuantity(item.id, item.quantity - 1)}
                              className="w-8 h-8 border rounded hover:bg-gray-100"
                            >
                              -
                            </button>
                            <span className="w-8 text-center">{item.quantity}</span>
                            <button
                              onClick={() => updateQuantity(item.id, item.quantity + 1)}
                              className="w-8 h-8 border rounded hover:bg-gray-100"
                            >
                              +
                            </button>
                            <button
                              onClick={() => removeFromCart(item.id)}
                              className="ml-auto text-red-500 hover:text-red-600"
                            >
                              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                              </svg>
                            </button>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              {/* Footer */}
              {cart.length > 0 && (
                <div className="border-t p-4 space-y-4">
                  <div className="flex justify-between text-lg font-bold">
                    <span>Tổng cộng:</span>
                    <span className="text-pink-500">{totalPrice.toLocaleString("vi-VN")}đ</span>
                  </div>
                  <button className="w-full bg-pink-500 text-white py-3 rounded-lg hover:bg-pink-600 font-medium">
                    Thanh toán
                  </button>
                  <button
                    onClick={clearCart}
                    className="w-full border border-gray-300 py-2 rounded-lg hover:bg-gray-50"
                  >
                    Xóa giỏ hàng
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
