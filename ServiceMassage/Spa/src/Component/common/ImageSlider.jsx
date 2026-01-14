import { useState, useEffect } from "react";

const images = [
  { src: "/Banner.jpg", alt: "Banner" },
  { src: "/logo.jpg", alt: "Logo" },
  { src: "/OIP.jpg", alt: "Spa Image" },
];

export default function ImageSlider() {
  const [currentIndex, setCurrentIndex] = useState(0);

  // Auto slide
  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentIndex((prev) => (prev + 1) % images.length);
    }, 4000);
    return () => clearInterval(timer);
  }, []);

  const goToSlide = (index) => setCurrentIndex(index);
  const prevSlide = () => setCurrentIndex((prev) => (prev - 1 + images.length) % images.length);
  const nextSlide = () => setCurrentIndex((prev) => (prev + 1) % images.length);

  return (
    <div></div>
  //   <section className="relative w-full h-[250px] md:h-[300px] overflow-hidden">
  //     {/* Slides */}
  //     <div
  //       className="flex transition-transform duration-500 ease-in-out h-full"
  //       style={{ transform: `translateX(-${currentIndex * 100}%)` }}
  //     >
  //       {images.map((img, index) => (
  //         <div key={index} className="min-w-full h-full relative">
  //           <img
  //             src={img.src}
  //             alt={img.alt}
  //             className="w-full h-full object-cover"
  //           />
  //           <div className="absolute inset-0 bg-black/30" />
  //         </div>
  //       ))}
  //     </div>

  //     {/* Navigation Arrows */}
  //     <button
  //       onClick={prevSlide}
  //       className="absolute left-4 top-1/2 -translate-y-1/2 bg-white/80 hover:bg-white p-3 rounded-full shadow-lg transition"
  //     >
  //       <svg className="w-6 h-6 text-gray-800" fill="none" stroke="currentColor" viewBox="0 0 24 24">
  //         <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
  //       </svg>
  //     </button>
  //     <button
  //       onClick={nextSlide}
  //       className="absolute right-4 top-1/2 -translate-y-1/2 bg-white/80 hover:bg-white p-3 rounded-full shadow-lg transition"
  //     >
  //       <svg className="w-6 h-6 text-gray-800" fill="none" stroke="currentColor" viewBox="0 0 24 24">
  //         <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
  //       </svg>
  //     </button>

  //     {/* Dots */}
  //     <div className="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2">
  //       {images.map((_, index) => (
  //         <button
  //           key={index}
  //           onClick={() => goToSlide(index)}
  //           className={`w-3 h-3 rounded-full transition ${
  //             index === currentIndex ? "bg-pink-500 w-8" : "bg-white/70 hover:bg-white"
  //           }`}
  //         />
  //       ))}
  //     </div>
  //   </section>
  );
}
