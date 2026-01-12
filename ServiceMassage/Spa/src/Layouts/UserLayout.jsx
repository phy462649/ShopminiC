import { Outlet } from "react-router-dom";
import HeaderHomePage from "../Component/HeaderHomePage";

export default function UserLayout() {
  return (
    <div className="min-h-screen flex flex-col">
      <HeaderHomePage />
      <main className="flex-1">
        <Outlet />
      </main>
      {/* Footer */}
      <footer className="bg-gray-800 text-white py-10">
        <div className="max-w-7xl mx-auto px-6">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            <div>
              <h3 className="font-bold text-lg mb-4">SPA Beauty & Health</h3>
              <p className="text-gray-400 text-sm">
                Nơi mang đến cho bạn sự thư giãn và làm đẹp hoàn hảo
              </p>
            </div>
            <div>
              <h3 className="font-bold mb-4">Dịch vụ</h3>
              <ul className="space-y-2 text-gray-400 text-sm">
                <li>Massage</li>
                <li>Chăm sóc da</li>
                <li>Chăm sóc body</li>
                <li>Gội đầu dưỡng sinh</li>
              </ul>
            </div>
            <div>
              <h3 className="font-bold mb-4">Liên kết</h3>
              <ul className="space-y-2 text-gray-400 text-sm">
                <li>Trang chủ</li>
                <li>Giới thiệu</li>
                <li>Liên hệ</li>
                <li>Đặt lịch</li>
              </ul>
            </div>
            <div>
              <h3 className="font-bold mb-4">Liên hệ</h3>
              <ul className="space-y-2 text-gray-400 text-sm">
                <li>📍 123 Nguyễn Văn Linh, Q7</li>
                <li>📞 0123 456 789</li>
                <li>✉️ contact@spabeauty.com</li>
              </ul>
            </div>
          </div>
          <div className="border-t border-gray-700 mt-8 pt-6 text-center text-gray-400 text-sm">
            © 2024 SPA Beauty & Health. All rights reserved.
          </div>
        </div>
      </footer>
    </div>
  );
}
