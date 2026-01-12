export default function About() {
  const team = [
    { name: "Nguyễn Thị A", role: "Giám đốc", image: "/OIP.jpg" },
    { name: "Trần Văn B", role: "Chuyên gia Massage", image: "/OIP.jpg" },
    { name: "Lê Thị C", role: "Chuyên gia Skincare", image: "/OIP.jpg" },
    { name: "Phạm Văn D", role: "Chuyên gia Tóc", image: "/OIP.jpg" },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Hero */}
      <section className="bg-gradient-to-r from-pink-100 to-purple-100 py-16">
        <div className="max-w-7xl mx-auto px-6 text-center">
          <h1 className="text-4xl font-bold text-gray-800 mb-4">Về chúng tôi</h1>
          <p className="text-lg text-gray-600">
            SPA Beauty & Health - Nơi mang đến vẻ đẹp và sức khỏe cho bạn
          </p>
        </div>
      </section>

      {/* Story */}
      <section className="py-16 max-w-7xl mx-auto px-6">
        <div className="grid md:grid-cols-2 gap-10 items-center">
          <div>
            <h2 className="text-3xl font-bold text-gray-800 mb-4">Câu chuyện của chúng tôi</h2>
            <p className="text-gray-600 mb-4">
              Được thành lập từ năm 2015, SPA Beauty & Health đã trở thành địa chỉ tin cậy 
              của hàng nghìn khách hàng trong việc chăm sóc sức khỏe và làm đẹp.
            </p>
            <p className="text-gray-600 mb-4">
              Với đội ngũ chuyên gia giàu kinh nghiệm và trang thiết bị hiện đại, 
              chúng tôi cam kết mang đến cho bạn những trải nghiệm tuyệt vời nhất.
            </p>
            <p className="text-gray-600">
              Sứ mệnh của chúng tôi là giúp mỗi khách hàng tìm lại sự cân bằng, 
              thư giãn và tự tin với vẻ đẹp của chính mình.
            </p>
          </div>
          <div>
            <img src="/OIP.jpg" alt="Spa" className="rounded-lg shadow-lg w-full" />
          </div>
        </div>
      </section>

      {/* Stats */}
      <section className="bg-pink-500 py-12">
        <div className="max-w-7xl mx-auto px-6">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-6 text-center text-white">
            <div>
              <p className="text-4xl font-bold">8+</p>
              <p>Năm kinh nghiệm</p>
            </div>
            <div>
              <p className="text-4xl font-bold">10,000+</p>
              <p>Khách hàng</p>
            </div>
            <div>
              <p className="text-4xl font-bold">50+</p>
              <p>Dịch vụ</p>
            </div>
            <div>
              <p className="text-4xl font-bold">20+</p>
              <p>Chuyên gia</p>
            </div>
          </div>
        </div>
      </section>

      {/* Team */}
      <section className="py-16 max-w-7xl mx-auto px-6">
        <h2 className="text-3xl font-bold text-center text-gray-800 mb-10">Đội ngũ của chúng tôi</h2>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
          {team.map((member, index) => (
            <div key={index} className="text-center">
              <img
                src={member.image}
                alt={member.name}
                className="w-32 h-32 rounded-full mx-auto mb-4 object-cover"
              />
              <h3 className="font-semibold text-gray-800">{member.name}</h3>
              <p className="text-gray-500 text-sm">{member.role}</p>
            </div>
          ))}
        </div>
      </section>
    </div>
  );
}
