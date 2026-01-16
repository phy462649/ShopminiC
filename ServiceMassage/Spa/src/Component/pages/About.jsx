export default function About() {
  const team = [
    { name: "Nguyễn Thị A", role: "Giám đốc", image: "/nv1.png" },
    { name: "Trần Văn B", role: "Chuyên gia Massage", image: "/nv2.png" },
    { name: "Lê Thị C", role: "Chuyên gia Skincare", image: "/nv3.png" },
    { name: "Phạm Văn D", role: "Chuyên gia Tóc", image: "/Screenshot 2026-01-15 220408.png" },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Banner */}
      <section className="relative h-[300px] md:h-[400px]">
        <img 
          src="/banner_gioithieu.jpg" 
          alt="Giới thiệu SPA Beauty" 
          className="w-full h-full object-cover"
        />
        <div className="absolute inset-0 bg-black/40 flex items-center justify-center">
          <div className="text-center text-white">
            <h1 className="text-4xl md:text-5xl font-bold mb-4">Về chúng tôi</h1>
            <p className="text-lg md:text-xl">SPA Beauty & Health - Nơi mang đến vẻ đẹp và sức khỏe cho bạn</p>
          </div>
        </div>
      </section>

      {/* Spa Introduction Section */}
      <section className="py-24 bg-pink-50">
        <div className="max-w-7xl mx-auto px-6">
          <div className="grid md:grid-cols-2 gap-16 items-center">
            {/* Left Content */}
            <div>
              <h2 className="text-5xl md:text-6xl font-serif text-pink-600 mb-10 leading-relaxed">
                Spa là gì?
              </h2>
              
              {/* Decorative lotus */}
              <div className="text-pink-400 text-4xl mb-10">❀</div>
              
              <p className="text-pink-700 text-xl mb-8 leading-relaxed">
                Spa không chỉ là nơi để thư giãn, mà còn là không gian tái tạo năng lượng cho cả cơ thể và tâm trí. 
                Tại đây, bạn được chăm sóc bằng các liệu pháp trị liệu chuyên sâu như massage, xông hơi, 
                chăm sóc da mặt và cơ thể, giúp giảm căng thẳng, cải thiện sức khỏe và làm đẹp từ bên trong.
              </p>
              
              <p className="text-pink-700 text-xl leading-relaxed">
                Đến spa là chọn yêu thương chính mình – bởi khi bạn khỏe mạnh và thư thái, 
                bạn tỏa sáng theo cách riêng nhất!
              </p>
            </div>

            {/* Right Images Grid */}
            <div className="grid grid-cols-2 gap-6">
              <div className="space-y-6">
                <img 
                  src="/Screenshot 2026-01-15 220420.png" 
                  alt="Spa Reception" 
                  className="w-full h-64 object-cover rounded-2xl shadow-xl"
                />
                <img 
                  src="/Screenshot 2026-01-15 220427.png" 
                  alt="Lotus Spa" 
                  className="w-full h-64 object-cover rounded-2xl shadow-xl"
                />
              </div>
              <div className="space-y-6 pt-12">
                <img 
                  src="/Screenshot 2026-01-15 220434.png" 
                  alt="Spa Treatment" 
                  className="w-full h-64 object-cover rounded-2xl shadow-xl"
                />
                {/* Years Badge */}
                <div className="bg-pink-600 text-white rounded-2xl p-10 text-center shadow-xl">
                  <p className="text-6xl font-bold font-serif"></p>
                  <div className="w-px h-12 bg-white/50 mx-auto my-4"></div>
                  <h4>21 năm cống h</h4>
                  <p className="text-lg">cống hiến cho khách hàng</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* 5 Senses Benefits Section */}
      <section className="py-24 bg-white">
        <div className="max-w-7xl mx-auto px-6">
          <div className="grid md:grid-cols-3 gap-16">
            {/* Left Column - Title */}
            <div>
              <h2 className="text-5xl md:text-6xl font-serif text-pink-600 mb-10">Spa mang lại lợi ích gì?</h2>
              <div className="text-pink-400 text-4xl mb-10">❀</div>
              <p className="text-pink-700 text-xl leading-relaxed">
                Hiểu theo cách nguyên bản, SPA là liệu pháp chăm sóc sức khỏe có sử dụng nước (còn gọi là "thủy trị liệu"). 
                Ngày nay SPA được hiểu là các liệu pháp chăm sóc sức khỏe và sắc đẹp giúp cân bằng cơ thể và tinh thần. 
                Một nơi được gọi là SPA đúng nghĩa phải tác động đến 5 giác quan mới có khả năng tái tạo nguồn năng lượng 
                giúp khách hàng khỏe mạnh và tươi trẻ hơn như tại Sen Spa.
              </p>
            </div>

            {/* Middle Column */}
            <div className="space-y-12">
              {/* Khứu giác */}
              <div className="flex gap-6">
                <div className="w-24 h-24 bg-pink-600 rounded-2xl flex items-center justify-center flex-shrink-0">
                  <svg className="w-12 h-12 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19.428 15.428a2 2 0 00-1.022-.547l-2.387-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z" />
                  </svg>
                </div>
                <div>
                  <h3 className="text-2xl font-bold text-pink-600 mb-4">Khứu giác</h3>
                  <p className="text-pink-700 text-lg leading-relaxed">
                    Mùi ngửi thấy mùi thơm dễ chịu của cây cỏ, tinh dầu tự nhiên dịu nhẹ khi vừa bước vào và bao phủ toàn bộ không gian các tầng của spa.
                  </p>
                </div>
              </div>

              {/* Vị giác */}
              <div className="flex gap-6">
                <div className="w-24 h-24 bg-pink-600 rounded-2xl flex items-center justify-center flex-shrink-0">
                  <svg className="w-12 h-12 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <div>
                  <h3 className="text-2xl font-bold text-pink-600 mb-4">Vị giác</h3>
                  <p className="text-pink-700 text-lg leading-relaxed">
                    Menu thức uống đặc biệt của Sen Spa không những thơm ngon mà còn rất tốt cho sức khỏe và làn da.
                  </p>
                </div>
              </div>

              {/* Xúc giác */}
              <div className="flex gap-6">
                <div className="w-24 h-24 bg-pink-600 rounded-2xl flex items-center justify-center flex-shrink-0">
                  <svg className="w-12 h-12 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 11.5V14m0-2.5v-6a1.5 1.5 0 113 0m-3 6a1.5 1.5 0 00-3 0v2a7.5 7.5 0 0015 0v-5a1.5 1.5 0 00-3 0m-6-3V11m0-5.5v-1a1.5 1.5 0 013 0v1m0 0V11m0-5.5a1.5 1.5 0 013 0v3m0 0V11" />
                  </svg>
                </div>
                <div>
                  <h3 className="text-2xl font-bold text-pink-600 mb-4">Xúc giác</h3>
                  <p className="text-pink-700 text-lg leading-relaxed">
                    Khách hàng được vuốt ve chăm sóc làn da sáng mịn và loại bỏ các điểm tắc nghẽn năng lượng giúp cơ thể bừng sức sống.
                  </p>
                </div>
              </div>
            </div>

            {/* Right Column */}
            <div className="space-y-12">
              {/* Thị giác */}
              <div className="flex gap-6">
                <div className="w-24 h-24 bg-pink-600 rounded-2xl flex items-center justify-center flex-shrink-0">
                  <svg className="w-12 h-12 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                </div>
                <div>
                  <h3 className="text-2xl font-bold text-pink-600 mb-4">Thị giác</h3>
                  <p className="text-pink-700 text-lg leading-relaxed">
                    Cách trang trí tối giản cùng ánh sáng dịu nhẹ đưa tinh thần ta vào sự tĩnh lặng để đạt sự thư giãn thật sâu, thật thoải mái.
                  </p>
                </div>
              </div>

              {/* Thính giác */}
              <div className="flex gap-6">
                <div className="w-24 h-24 bg-pink-600 rounded-2xl flex items-center justify-center flex-shrink-0">
                  <svg className="w-12 h-12 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19V6l12-3v13M9 19c0 1.105-1.343 2-3 2s-3-.895-3-2 1.343-2 3-2 3 .895 3 2zm12-3c0 1.105-1.343 2-3 2s-3-.895-3-2 1.343-2 3-2 3 .895 3 2zM9 10l12-3" />
                  </svg>
                </div>
                <div>
                  <h3 className="text-2xl font-bold text-pink-600 mb-4">Thính giác</h3>
                  <p className="text-pink-700 text-lg leading-relaxed">
                    Âm nhạc du dương chuyên biệt dành cho spa giúp trị liệu tâm hồn, xóa tan căng thẳng và phiền muộn mang đến giấc ngủ sâu.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Story */}
      <section className="py-16 max-w-7xl mx-auto px-6">
        <div className="grid md:grid-cols-2 gap-10 items-center">
<div>
  <h2 className="text-4xl font-bold text-pink-500 mb-8">
    Câu chuyện của chúng tôi
  </h2>

  <p className="text-lg text-pink-400 mb-5 leading-relaxed">
    Được thành lập từ năm 2015, SPA Beauty & Health ra đời với mong muốn mang đến 
    một không gian chăm sóc sức khỏe và sắc đẹp toàn diện, nơi mỗi khách hàng 
    có thể tạm gác lại những bộn bề của cuộc sống để lắng nghe cơ thể và cảm xúc 
    của chính mình.
  </p>

  <p className="text-lg text-pink-400 mb-5 leading-relaxed">
    Ngay từ những ngày đầu, chúng tôi đã xác định chất lượng dịch vụ và trải nghiệm 
    của khách hàng là giá trị cốt lõi. SPA Beauty & Health không chỉ đầu tư vào 
    hệ thống trang thiết bị hiện đại, công nghệ làm đẹp tiên tiến mà còn đặc biệt 
    chú trọng đến yếu tố con người – đội ngũ chuyên gia, kỹ thuật viên được đào tạo 
    bài bản, giàu kinh nghiệm và luôn tận tâm trong từng liệu trình.
  </p>

  <p className="text-lg text-pink-400 mb-5 leading-relaxed">
    Mỗi dịch vụ tại SPA Beauty & Health đều được nghiên cứu và thiết kế dựa trên 
    nhu cầu thực tế của khách hàng, kết hợp hài hòa giữa phương pháp chăm sóc 
    truyền thống và xu hướng làm đẹp hiện đại. Chúng tôi tin rằng, vẻ đẹp bền vững 
    chỉ có thể được nuôi dưỡng khi sức khỏe thể chất và tinh thần được cân bằng.
  </p>

  <p className="text-lg text-pink-400 mb-5 leading-relaxed">
    Trải qua nhiều năm phát triển, SPA Beauty & Health vinh dự trở thành người bạn 
    đồng hành đáng tin cậy của hàng nghìn khách hàng trong hành trình chăm sóc 
    bản thân. Sự hài lòng, tin tưởng và những nụ cười rạng rỡ sau mỗi liệu trình 
    chính là động lực để chúng tôi không ngừng hoàn thiện và nâng cao chất lượng 
    dịch vụ mỗi ngày.
  </p>

  <p className="text-lg text-pink-400 leading-relaxed">
    Với sứ mệnh giúp mỗi khách hàng tìm lại sự cân bằng từ bên trong, thư giãn trọn vẹn 
    và tự tin tỏa sáng với vẻ đẹp tự nhiên của chính mình, SPA Beauty & Health cam kết 
    tiếp tục đổi mới, sáng tạo và mang đến những giá trị tốt đẹp, bền vững cho cộng đồng.
  </p>
</div>

          <div>
            <img src="/aboutwe.jpg" alt="Spa" className="rounded-lg shadow-lg w-full" />
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

      {/* Awards Section */}
      <section className="py-16 bg-gray-50">
        <div className="max-w-7xl mx-auto px-6">
          <div className="grid md:grid-cols-4 gap-8 items-center">
            {/* Left Title */}
            <div>
              <h2 className="text-4xl font-serif text-pink-600 mb-4">Giải thưởng</h2>
              <div className="flex items-center gap-2 mb-4">
                <div className="w-8 h-px bg-pink-400"></div>
                <span className="text-pink-400 text-xl">❀</span>
                <div className="w-8 h-px bg-pink-400"></div>
              </div>
              <p className="text-pink-700 text-sm leading-relaxed">
                Sen Spa nhiều năm liền được vinh danh là Spa được yêu thích nhất tại thành phố Hồ Chí Minh
              </p>
            </div>

            {/* Awards */}
            <div className="text-center">
              <img src="/award2012.jpg" alt="Award 1" className="w-32 h-40 object-contain mx-auto mb-4" />
              <p className="text-pink-700 text-sm font-medium">Giải thưởng The Guide Awards (2012)</p>
            </div>
            <div className="text-center">
              <img src="/Screenshot 2026-01-15 220500.png" alt="Award 2" className="w-32 h-40 object-contain mx-auto mb-4" />
              <p className="text-pink-700 text-sm font-medium">Năm 2012 Sen Spa đạt Top 5 Spa đạt chuẩn "TP.HCM 100 điều thú vị" lần II.</p>
            </div>
            <div className="text-center">
              <img src="/Screenshot 2026-01-15 220512.png" alt="Award 3" className="w-32 h-40 object-contain mx-auto mb-4" />
              <p className="text-pink-700 text-sm font-medium">Giải thưởng Top 5 Spa đạt chuẩn cuộc bình chọn "TP.HCM 100 điều thú vị" lần I (2009)</p>
            </div>
          </div>

          {/* Navigation Arrows */}
          <div className="flex justify-center gap-4 mt-8">
            <button className="w-10 h-10 border border-pink-400 rounded-full flex items-center justify-center text-pink-400 hover:bg-pink-50">
              ←
            </button>
            <button className="w-10 h-10 border border-pink-400 rounded-full flex items-center justify-center text-pink-400 hover:bg-pink-50">
              →
            </button>
          </div>
        </div>
      </section>

      {/* Facilities Section */}
      <section className="py-0">
        <div className="grid grid-cols-4 grid-rows-2">
          {/* Title Card */}
          <div className="bg-pink-600 text-white p-8 flex flex-col justify-center">
            <h2 className="text-3xl font-serif mb-4">Cơ sở vật chất</h2>
            <div className="flex items-center gap-2 mb-4">
              <div className="w-6 h-px bg-white/50"></div>
              <span className="text-white/80 text-lg">❀</span>
              <div className="w-6 h-px bg-white/50"></div>
            </div>
            <p className="text-white/90 text-sm leading-relaxed">
              Khách hàng tận hưởng sự thoải mái riêng tư với 6 tầng riêng biệt.
            </p>
          </div>

          {/* Floor Images Row 1 */}
          <div className="relative group overflow-hidden">
            <img src="/Screenshot 2026-01-15 220633.png" alt="Tầng 1" className="w-full h-full object-cover group-hover:scale-105 transition" />
            <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/70 to-transparent p-4">
              <span className="text-white font-medium">Tầng 1</span>
            </div>
          </div>
          <div className="relative group overflow-hidden">
            <img src="/Screenshot 2026-01-15 220645.png" alt="Tầng 2" className="w-full h-full object-cover group-hover:scale-105 transition" />
            <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/70 to-transparent p-4">
              <span className="text-white font-medium">Tầng 2</span>
            </div>
          </div>
          <div className="relative group overflow-hidden">
            <img src="/Screenshot 2026-01-15 220702.png" alt="Tầng 3" className="w-full h-full object-cover group-hover:scale-105 transition" />
            <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/70 to-transparent p-4">
              <span className="text-white font-medium">Tầng 3</span>
            </div>
          </div>

          {/* Floor Images Row 2 */}
          <div className="relative group overflow-hidden">
            <img src="/Screenshot 2026-01-15 220729.png" alt="Tầng 4" className="w-full h-full object-cover group-hover:scale-105 transition" />
            <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/70 to-transparent p-4">
              <span className="text-white font-medium">Tầng 4</span>
            </div>
          </div>
          <div className="relative group overflow-hidden">
            <img src="/Screenshot 2026-01-15 220739.png" alt="Tầng 5" className="w-full h-full object-cover group-hover:scale-105 transition" />
            <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/70 to-transparent p-4">
              <span className="text-white font-medium">Tầng 5</span>
            </div>
          </div>
          <div className="relative group overflow-hidden">
            <img src="/Screenshot 2026-01-15 220800.png" alt="Tầng 6" className="w-full h-full object-cover group-hover:scale-105 transition" />
            <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/70 to-transparent p-4">
              <span className="text-white font-medium">Tầng 6</span>
            </div>
          </div>

          {/* Info Card */}
          <div className="bg-pink-50 p-6 flex flex-col justify-center">
            <span className="text-pink-600 font-medium text-sm mb-2">Tầng 7</span>
            <p className="text-pink-700 text-sm leading-relaxed mb-4">
              Khu căn hộ dịch vụ với đầy đủ tiện nghi dành cho khách lưu trú dài hạn và thuận tiện di chuyển khu vực trung tâm.
            </p>
            <button className="w-10 h-10 border border-pink-400 rounded-full flex items-center justify-center text-pink-400 hover:bg-pink-100 self-end">
              →
            </button>
          </div>
        </div>
      </section>
    </div>
  );
}
