export default {
  purge: ["./src/**/*.{js,jsx,ts,tsx}", "./public/index.html"],
  darkMode: false,
  theme: {
    extend: {
      keyframes: {
        fadeSlide: {
          "0%": { opacity: "0", transform: "translateX(-40px)" },
          "100%": { opacity: "1", transform: "translateX(0)" },
        },
        zoom: {
          "0%": { transform: "scale(0.8)", opacity: "0" },
          "100%": { transform: "scale(1)", opacity: "1" },
        },
        stagger: {
          "0%": { opacity: "0", transform: "translateY(8px)" },
          "100%": { opacity: "1", transform: "translateY(0)" },
        },
        fadeUp: {
          "0%": { opacity: "0", transform: "translateY(20px)" },
          "100%": { opacity: "1", transform: "translateY(0)" },
        },
      },

      animation: {
        fadeSlide: "fadeSlide 0.5s ease-out",
        zoom: "zoom 0.5s ease-out",
        stagger: "stagger 0.4s ease-out forwards",
        fadeUp: "fadeUp 0.6s ease-out",
      },
    },
  },
  variants: { extend: {} },
  plugins: [],
};
