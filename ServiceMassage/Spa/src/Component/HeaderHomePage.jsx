import { useState, useRef, useEffect } from "react";
import logo from "../../public/OIP.jpg";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../features/Auth/UseAuth";
import { authService } from "../services";

export default function 
HeaderHomePage() {
  const navigate = useNavigate();
  const { isAuthenticated, logout } = useAuth();
  const [showDropdown, setShowDropdown] = useState(false);
  const dropdownRef = useRef(null);

  // Lấy thông tin user từ localStorage
  const user = authService.getCurrentUser();

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setShowDropdown(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const handleLogout = () => {
    logout();
    setShowDropdown(false);
    navigate("/");
  };

  return (
    <header className="bg-white border-b sticky top-0 z-40">
      <div className="max-w-7xl mx-auto flex items-center justify-between px-6">
        {/* Logo */}
        <div className="flex items-center gap-3">
          <img src={logo} alt="SPA Beauty & Health" className="w-24 h-28" />
        </div>

        {/* Menu */}
        <nav aria-label="Main navigation" className="flex items-center gap-6 mt-4">
          <ul className="flex gap-6 text-gray-700 font-medium">
            {[
              { name: "Trang chủ", path: "/" },
              { name: "Dịch vụ", path: "/services" },
              { name: "Sản phẩm", path: "/products" },
              { name: "Giới thiệu", path: "/about" },
              { name: "Liên hệ", path: "/contact" },
            ].map((item) => (
              <li key={item.name}>
                <button
                  onClick={() => navigate(item.path)}
                  className="hover:text-pink-500 focus:outline-none focus-visible:ring-2 focus-visible:ring-pink-400 rounded"
                >
                  {item.name}
                </button>
              </li>
            ))}
          </ul>

          {/* User Section */}
          {isAuthenticated ? (
            <div className="relative mb-4" ref={dropdownRef}>
              <button
                onClick={() => setShowDropdown(!showDropdown)}
                className="flex items-center gap-2 px-4 py-2 rounded-lg hover:bg-gray-100 transition-all duration-300"
              >
                {/* User Avatar */}
                <div className="w-10 h-10 rounded-full bg-pink-500 flex items-center justify-center text-white font-semibold">
                  {user?.name?.charAt(0)?.toUpperCase() || "U"}
                </div>
                {/* User Name */}
                <span className="text-gray-700 font-medium max-w-[120px] truncate">
                  {user?.name || "Người dùng"}
                </span>
                {/* Dropdown Arrow */}
                <svg
                  className={`w-4 h-4 text-gray-500 transition-transform ${showDropdown ? "rotate-180" : ""}`}
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                </svg>
              </button>

              {/* Dropdown Menu */}
              {showDropdown && (
                <div className="absolute right-0 mt-2 w-56 bg-white rounded-lg shadow-lg border py-2 z-50">
                  {/* User Info */}
                  <div className="px-4 py-3 border-b">
                    <p className="text-sm font-semibold text-gray-800">{user?.name || "Người dùng"}</p>
                    <p className="text-xs text-gray-500 truncate">{user?.email || ""}</p>
                  </div>

                  {/* Menu Items */}
                  <button
                    onClick={() => { navigate("/profile"); setShowDropdown(false); }}
                    className="w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-100 flex items-center gap-3"
                  >
                    <svg className="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                    </svg>
                    Tài khoản của tôi
                  </button>

                  <button
                    onClick={() => { navigate("/orders"); setShowDropdown(false); }}
                    className="w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-100 flex items-center gap-3"
                  >
                    <svg className="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                    </svg>
                    Đơn hàng
                  </button>

                  <button
                    onClick={() => { navigate("/bookings"); setShowDropdown(false); }}
                    className="w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-100 flex items-center gap-3"
                  >
                    <svg className="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                    Lịch đặt
                  </button>
                  {user?.role === "Admin" && (
                    <button
                      onClick={() => { navigate("/admin"); setShowDropdown(false); }}
                      className="w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-100 flex items-center gap-3"
                    >
                      <svg className="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      </svg>
                      Quản trị
                    </button>
                  )}
                  <div className="border-t my-1"></div>

                  <button
                    onClick={handleLogout}
                    className="w-full px-4 py-2 text-left text-sm text-red-600 hover:bg-red-50 flex items-center gap-3"
                  >
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                    </svg>
                    Đăng xuất
                  </button>
                </div>
              )}
            </div>
          ) : (
            <button
              onClick={() => navigate("/login")}
              className="bg-pink-500 text-white px-4 py-2 rounded-md mb-4 hover:bg-pink-600 transition-all duration-300 h-12 flex items-center justify-center"
            >
              Đăng nhập
            </button>
          )}
        </nav>
      </div>
    </header>
  );
}
