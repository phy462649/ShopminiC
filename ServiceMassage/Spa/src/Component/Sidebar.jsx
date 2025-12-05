export default function Sidebar({ items, active, onSelect }) {
  return (
    <aside className="w-64 h-screen pl-4 fixed top-0 left-0 bg-white flex flex-col">
      <nav className="space-y-1 flex-1">
        {items.map((item) => {
          if (item.isLogo) {
            return (
              <div
                key={item.id}
                className="h-24 flex items-center justify-center border-b mb-2"
              >
                <img
                  src="/OIP.jpg"
                  alt="Logo"
                  className="h-16 w-auto object-contain"
                />
              </div>
            );
          }
          return (
            <button
              key={item.id}
              onClick={() => onSelect(item.id)}
              className={`block w-full text-left px-4 py-2 rounded-xl hover:bg-gray-100 transition
                ${active === item.id ? "bg-gray-200 font-semibold" : ""}
                ${item.hidden ? "invisible" : ""}`}
            >
              {item.label}
            </button>
          );
        })}
      </nav>

      <button
        className="fixed w-48 left-0 bottom-0 m-6 px-4 py-2 bg-pink-600 
                         text-white rounded-md hover:bg-pink-700 shadow-lg"
      >
        Logout
      </button>
    </aside>
  );
}
