import logo from "../../public/OIP.jpg";
export default function HeaderHomePage() {
  return (
    <header className="bg-white border-b sticky top-0 z-40 ">
      <div className="max-w-7xl mx-auto flex items-center justify-between px-6">
        {/* Logo */}
        <div className="flex items-center gap-3">
          <img src={logo} alt="SPA Beauty & Health" className="w-20 h-auto" />
        </div>

        {/* Menu */}
        <nav aria-label="Main navigation">
          <ul className="flex gap-6 text-gray-700 font-medium">
            {["Trang chủ", "Dịch vụ", "Product", "Giới thiệu", "Liên hệ"].map(
              (item) => (
                <li key={item}>
                  <button
                    role="link"
                    className="hover:text-pink-500 focus:outline-none focus-visible:ring-2 focus-visible:ring-pink-400 rounded"
                  >
                    {item}
                  </button>
                </li>
              )
            )}
          </ul>
        </nav>
      </div>
    </header>
  );
}
