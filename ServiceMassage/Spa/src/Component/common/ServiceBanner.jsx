export default function ServiceBanner() {
  const services = [
    {
      title: "Beauty",
      subtitle: "Spa",
      description: "Dich vu cham soc da mat chuyen sau, giup lan da tro nen min mang va rang ro.",
      image: "/OIP.jpg",
      website: "www.beautyspa.com",
      leafPosition: "left",
    },
    {
      title: "Beauty Center",
      subtitle: "Spa",
      description: "Trung tam lam dep hang dau voi cac lieu trinh duoc ca nhan hoa theo nhu cau.",
      image: "/Banner.jpg",
      website: "www.beautyspa.com",
      leafPosition: "bottom",
    },
    {
      title: "Beauty",
      subtitle: "Spa",
      description: "Massage thu gian, giai toa cang thang va mang lai cam giac thu thai.",
      image: "/OIP.jpg",
      website: "www.beautyspa.com",
      leafPosition: "right",
    },
    {
      title: "Beauty",
      subtitle: "Spa",
      description: "Cham soc toan dien tu da mat den co the voi san pham thien nhien.",
      image: "/logo.jpg",
      website: "www.beautyspa.com",
      leafPosition: "corner",
    },
  ];

  return (
    <section className="py-12 bg-[#f8e8e0]">
      <div className="max-w-7xl mx-auto px-6">
        <h2 className="text-3xl font-bold text-center text-gray-800 mb-10">
          Dich vu cua chung toi
        </h2>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {services.map((service, index) => (
            <div
              key={index}
              className="bg-white/90 rounded-lg p-6 relative overflow-hidden shadow-lg hover:shadow-xl transition-shadow"
            >
              {/* Decorative leaves - Top */}
              <svg className="absolute top-0 left-0 w-20 h-20 text-green-400 opacity-60" viewBox="0 0 100 100">
                <path fill="currentColor" d="M10,50 Q30,20 50,10 Q40,40 10,50" />
                <path fill="currentColor" d="M15,60 Q35,35 55,25 Q45,50 15,60" opacity="0.7" />
              </svg>

              {/* Spa label */}
              <div className="text-center mb-4 relative z-10">
                <span className="text-pink-400 text-sm italic flex items-center justify-center gap-1">
                  <span className="text-pink-300">&#10084;</span> {service.subtitle}
                </span>
              </div>

              {/* Title */}
              <h3 className="text-3xl font-serif text-gray-800 text-center mb-4">
                {service.title}
              </h3>

              {/* Circular Image */}
              <div className="flex justify-center mb-4">
                <div className="w-28 h-28 rounded-full overflow-hidden border-4 border-pink-100 shadow-md">
                  <img
                    src={service.image}
                    alt={service.title}
                    className="w-full h-full object-cover"
                  />
                </div>
              </div>

              {/* Description */}
              <p className="text-gray-500 text-sm text-center mb-4 leading-relaxed">
                {service.description}
              </p>

              {/* Website */}
              <p className="text-gray-400 text-xs text-center italic">
                {service.website}
              </p>

              {/* Decorative leaves - Bottom */}
              <svg className="absolute bottom-0 right-0 w-24 h-24 text-pink-200 opacity-50" viewBox="0 0 100 100">
                <path fill="currentColor" d="M90,90 Q70,60 80,40 Q85,70 90,90" />
                <path fill="currentColor" d="M80,95 Q55,70 65,45 Q75,75 80,95" opacity="0.6" />
              </svg>
              
              {/* Green leaf decoration */}
              <svg className="absolute bottom-2 left-2 w-16 h-16 text-green-400 opacity-40" viewBox="0 0 100 100">
                <path fill="currentColor" d="M20,80 Q40,50 30,20 Q50,50 20,80" />
              </svg>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
