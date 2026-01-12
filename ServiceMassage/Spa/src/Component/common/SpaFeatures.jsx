import { useNavigate } from "react-router-dom";

export default function SpaFeatures() {
  const navigate = useNavigate();

  return (
    <section className="py-12 bg-white">
      <div className="max-w-7xl mx-auto px-6">
        {/* Grid Layout */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          {/* Left - Pool Sauna */}
          <div className="md:row-span-2">
            <div className="relative h-full min-h-[400px] rounded-lg overflow-hidden">
              <img
                src="/OIP.jpg"
                alt="Pool Sauna"
                className="w-full h-full object-cover"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent" />
              <div className="absolute bottom-0 left-0 right-0 p-6 text-center text-white">
                <h3 className="text-xl font-bold uppercase">Detox in our</h3>
                <p className="text-pink-400 font-bold text-lg italic">Pool Sauna</p>
                <p className="text-sm mt-2 text-gray-200">
                  Seamlessly empower fully researched growth strategies and interoperable internal or organic sources.
                </p>
              </div>
            </div>
          </div>

          {/* Center Top - Treat Your Inner Self */}
          <div className="bg-[#c4727a] rounded-lg p-6 text-center text-white flex flex-col justify-center">
            <h3 className="text-xl font-bold uppercase italic">Treat Your</h3>
            <p className="text-2xl font-bold uppercase">Inner Self</p>
            <p className="text-sm mt-4 text-gray-100">
              We believe that the body is amazing and given the right tools and time is able to heal itself naturally.
            </p>
          </div>

          {/* Center Top - Endulge Your Soul */}
          <div className="bg-[#c4727a] rounded-lg p-6 text-center text-white flex flex-col justify-center">
            <h3 className="text-xl font-bold uppercase italic">Endulge</h3>
            <p className="text-2xl font-bold uppercase">Your Soul</p>
            <p className="text-sm mt-4 text-gray-100">
              Primavera is a holistic based wellness facility that offers unique services that are customized to each client's needs.
            </p>
          </div>

          {/* Right - Natural Balance */}
          <div className="md:row-span-2">
            <div className="relative h-full min-h-[400px] rounded-lg overflow-hidden">
              <img
                src="/OIP.jpg"
                alt="Natural Balance"
                className="w-full h-full object-cover"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent" />
              <div className="absolute bottom-0 left-0 right-0 p-6 text-center text-white">
                <h3 className="text-xl font-bold uppercase">Find Your</h3>
                <p className="text-pink-400 font-bold text-lg italic">Natural Balance</p>
                <p className="text-sm mt-2 text-gray-200">
                  Quickly cultivate optimal processes and tactical architectures. Completely iterate covalent strategic theme areas.
                </p>
              </div>
            </div>
          </div>

          {/* Center Bottom - Massage Image */}
          <div className="rounded-lg overflow-hidden">
            <img
              src="/OIP.jpg"
              alt="Massage Treatment"
              className="w-full h-48 object-cover"
            />
          </div>

          {/* Center Bottom - Spa Treatment Image */}
          <div className="rounded-lg overflow-hidden">
            <img
              src="/OIP.jpg"
              alt="Spa Treatment"
              className="w-full h-48 object-cover"
            />
          </div>
        </div>

        {/* Buttons */}
        <div className="flex justify-center gap-4 mt-8">
          <button
            onClick={() => navigate("/services")}
            className="px-8 py-3 border-2 border-gray-800 text-gray-800 rounded hover:bg-gray-800 hover:text-white transition font-medium"
          >
            Find out more
          </button>
          <button
            onClick={() => navigate("/booking")}
            className="px-8 py-3 bg-[#c4727a] text-white rounded hover:bg-[#b5636b] transition font-medium"
          >
            Find out more
          </button>
        </div>
      </div>
    </section>
  );
}
