import { useState, useRef, useEffect } from "react";

export default function HeaderAdmin({
  links = [],
  user = null,
  userLoading = false,
  userError = null,
  onSignOut,
}) {
  const [mobileOpen, setMobileOpen] = useState(false);
  const menuBtnRef = useRef(null);
  const mobileMenuRef = useRef(null);

  // Close on outside click + ESC
  useEffect(() => {
    const onClick = (e) => {
      if (
        mobileOpen &&
        mobileMenuRef.current &&
        !mobileMenuRef.current.contains(e.target) &&
        menuBtnRef.current &&
        !menuBtnRef.current.contains(e.target)
      ) {
        setMobileOpen(false);
      }
    };
    const onKey = (e) => {
      if (e.key === "Escape") setMobileOpen(false);
    };
    window.addEventListener("click", onClick);
    window.addEventListener("keydown", onKey);
    return () => {
      window.removeEventListener("click", onClick);
      window.removeEventListener("keydown", onKey);
    };
  }, [mobileOpen]);

  // Focus first element in mobile menu
  useEffect(() => {
    if (mobileOpen && mobileMenuRef.current) {
      const first = mobileMenuRef.current.querySelector(
        'button, [tabindex]:not([tabindex="-1"])'
      );
      first?.focus();
    }
  }, [mobileOpen]);

  return (
    <header className="bg-white sticky top-0 z-40 shadow-sm">
      <div className="mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header container */}
        <div className="flex items-center justify-end h-28">
          {/* Menu links - left */}
          <nav aria-label="Primary" className="hidden md:flex gap-3">
            {links.map((l) => (
              <button
                key={l.id}
                onClick={() => (window.location.href = l.href)}
                className="px-5 py-3 text-base font-medium text-gray-700 rounded-lg hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-pink-500"
              >
                {l.label}
              </button>
            ))}
          </nav>

          {/* Spacer for mobile (if needed) */}
          <div className="flex-1 md:hidden" />

          {/* Account buttons - right */}
          <div className="flex items-center gap-3">
            {userLoading ? (
              <div className="text-base text-gray-500">Loading...</div>
            ) : userError ? (
              <div className="text-base text-red-600">{userError}</div>
            ) : user ? (
              <>
                <img
                  src={
                    user.avatarUrl ||
                    `https://ui-avatars.com/api/?name=${encodeURIComponent(
                      user.name
                    )}`
                  }
                  alt=""
                  className="h-10 w-10 rounded-full object-cover"
                />
                <span className="hidden sm:inline text-base">{user.name}</span>
                <button
                  onClick={onSignOut}
                  className="px-5 py-3 text-base rounded-lg hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-pink-500"
                >
                  Sign out
                </button>
              </>
            ) : (
              <>
                <button
                  onClick={() => (window.location.href = "/signin")}
                  className="px-5 py-3 text-base rounded-lg border hover:bg-gray-50"
                >
                  Sign in
                </button>
                <button
                  onClick={() => (window.location.href = "/signup")}
                  className="px-5 py-3 text-base rounded-lg bg-pink-600 text-white hover:bg-pink-700"
                >
                  Sign up
                </button>
              </>
            )}

            {/* Mobile menu button */}
            <button
              ref={menuBtnRef}
              onClick={() => setMobileOpen((v) => !v)}
              aria-expanded={mobileOpen}
              aria-label={mobileOpen ? "Close menu" : "Open menu"}
              className="md:hidden p-3 rounded-lg hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-pink-500"
            >
              <svg className="h-6 w-6" fill="none" stroke="currentColor">
                {mobileOpen ? (
                  <path strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                ) : (
                  <path strokeWidth="2" d="M4 6h16M4 12h16M4 18h16" />
                )}
              </svg>
            </button>
          </div>
        </div>
      </div>

      {/* Mobile dropdown */}
      <div
        ref={mobileMenuRef}
        className={`md:hidden ${mobileOpen ? "block" : "hidden"} border-t`}
      >
        <div className="py-3 space-y-2">
          {links.map((l) => (
            <button
              key={l.id}
              onClick={() => (window.location.href = l.href)}
              className="block w-full text-left px-5 py-3 text-base rounded-lg hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-pink-500"
            >
              {l.label}
            </button>
          ))}
        </div>
      </div>
    </header>
  );
}
