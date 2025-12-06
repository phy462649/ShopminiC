export default function Sidebar({ items, active, onSelect }) {
  return (
    <aside className="fixed inset-y-0 left-0 w-72 bg-white border-r border-gray-200 shadow-xl flex flex-col">
      {/* Logo */}
      {items.find((item) => item.isLogo) && (
        <div className="h-24 flex items-center justify-center mb-6 animate-slideIn">
          <img
            src="/OIP.jpg"
            alt="Logo"
            className="h-20 w-auto object-contain drop-shadow-lg"
          />
        </div>
      )}

      {/* Navigation Items */}
      <nav className="flex-1 space-y-1.5 px-3 overflow-y-auto scrollbar-thin scrollbar-thumb-gray-300">
        {items.map((item) => {
          if (item.isLogo) return null;

          const isActive = active === item.id;

          return (
            <button
              key={item.id}
              onClick={() => onSelect(item.id)}
              aria-current={isActive ? "page" : undefined}
              className={`
                w-full h-12 px-4 rounded-xl flex items-center gap-3 text-left text-sm font-medium
                transition-all duration-300 ease-out group relative overflow-hidden
                animate-itemFade
                ${
                  isActive
                    ? "bg-blue-50 text-blue-700 shadow-sm ring-2 ring-blue-200 scale-[1.01]"
                    : "text-gray-700 hover:bg-gray-100 hover:text-gray-900 hover:shadow-sm hover:scale-[1.01]"
                }
              `}
            >
              {/* Ripple effect background */}
              <span className="absolute inset-0 bg-blue-400/10 scale-0 group-active:scale-150 transition-transform duration-500 origin-center rounded-full" />

              {/* Optional icon placeholder (nếu bạn muốn thêm icon sau) */}
              {item.icon && <item.icon className="w-5 h-5" />}

              <span className="relative z-10">{item.label}</span>

              {/* Active indicator */}
              {isActive && (
                <span className="absolute left-0 top-0 bottom-0 w-1 bg-pink-700 rounded-r-full" />
              )}
            </button>
          );
        })}
      </nav>

      {/* Logout Button */}
      <div className="p-4 border-t border-gray-200">
        <button
          className="w-full h-12 px-4 rounded-xl bg-red-500 text-white font-medium
                     hover:bg-red-600 active:bg-red-700 shadow-md
                     hover:shadow-lg active:scale-98 transition-all duration-200
                     flex items-center justify-center gap-2"
        >
          <svg
            className="w-5 h-5"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"
            />
          </svg>
          Logout
        </button>
      </div>
    </aside>
  );
}
