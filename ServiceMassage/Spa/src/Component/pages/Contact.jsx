import { useState } from "react";
import { message } from "antd";

export default function Contact() {
  const [form, setForm] = useState({ name: "", email: "", phone: "", content: "" });
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setLoading(true);
    setTimeout(() => {
      message.success("Gửi liên hệ thành công! Chúng tôi sẽ phản hồi sớm nhất.");
      setForm({ name: "", email: "", phone: "", content: "" });
      setLoading(false);
    }, 1000);
  };

  return (
    <div className="min-h-screen bg-gray-50 py-10">
      <div className="max-w-7xl mx-auto px-6">
        <h1 className="text-3xl font-bold text-center text-gray-800 mb-8">Liên hệ với chúng tôi</h1>

        <div className="grid md:grid-cols-2 gap-10">
          {/* Contact Info */}
          <div>
            <div className="bg-white rounded-lg shadow-md p-6 mb-6">
              <h2 className="text-xl font-semibold mb-4">Thông tin liên hệ</h2>
              <div className="space-y-4">
                <div className="flex items-start gap-3">
                  <span className="text-pink-500 text-xl">📍</span>
                  <div>
                    <p className="font-medium">Địa chỉ</p>
                    <p className="text-gray-600">123 Nguyễn Văn Linh, Quận 7, TP.HCM</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="text-pink-500 text-xl">📞</span>
                  <div>
                    <p className="font-medium">Điện thoại</p>
                    <p className="text-gray-600">0123 456 789</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="text-pink-500 text-xl">✉️</span>
                  <div>
                    <p className="font-medium">Email</p>
                    <p className="text-gray-600">contact@spabeauty.com</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <span className="text-pink-500 text-xl">🕐</span>
                  <div>
                    <p className="font-medium">Giờ làm việc</p>
                    <p className="text-gray-600">8:00 - 21:00 (Thứ 2 - Chủ nhật)</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Map */}
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-xl font-semibold mb-4">Bản đồ</h2>
              <div className="bg-gray-200 h-64 rounded flex items-center justify-center">
                <span className="text-gray-500">Google Map sẽ hiển thị ở đây</span>
              </div>
            </div>
          </div>

          {/* Contact Form */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-semibold mb-4">Gửi tin nhắn</h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-1">Họ tên *</label>
                <input
                  type="text"
                  name="name"
                  value={form.name}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-pink-500"
                  placeholder="Nhập họ tên"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Email *</label>
                <input
                  type="email"
                  name="email"
                  value={form.email}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-pink-500"
                  placeholder="Nhập email"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Số điện thoại</label>
                <input
                  type="tel"
                  name="phone"
                  value={form.phone}
                  onChange={handleChange}
                  className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-pink-500"
                  placeholder="Nhập số điện thoại"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Nội dung *</label>
                <textarea
                  name="content"
                  value={form.content}
                  onChange={handleChange}
                  required
                  rows={5}
                  className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-pink-500"
                  placeholder="Nhập nội dung tin nhắn"
                />
              </div>
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-pink-500 text-white py-3 rounded-lg hover:bg-pink-600 disabled:opacity-50"
              >
                {loading ? "Đang gửi..." : "Gửi tin nhắn"}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
