import { useState } from "react";
import HeaderAdmin from "./HeaderAdmin";
import Sidebar from "./Sidebar";
import Dashboard from "./pages/Dashboard";
import Users from "./pages/Users";
import Settings from "./pages/Settings";
import Logout from "./pages/Logout";

export default function AdminLayout() {
  const [page, setPage] = useState("dashboard");

  const renderContent = () => {
    switch (page) {
      case "dashboard":
        return <Dashboard />;
      case "users":
        return <Users />;
      case "settings":
        return <Settings />;
      case "logout":
        return <Logout />;
      default:
        return <div className="p-4">Page Not Found</div>;
    }
  };

  const sidebarItems = [
    { id: "logo", label: "", isLogo: true }, // <--- logo ở vị trí đầu

    { id: "dashboard", label: "Dashboard" },
    { id: "users", label: "Users" },
    { id: "settings", label: "Settings" },
    { id: "logout", label: "Logout" },
  ];

  return (
    <div className="flex h-screen overflow-hidden">
      <Sidebar items={sidebarItems} active={page} onSelect={setPage} />
      <div className="flex-1 flex flex-col ml-64">
        <HeaderAdmin
        //   user={{ name: "Admin", avatarUrl: "" }}
        //   onSignOut={() => setPage("logout")}
        />
        <main className="flex-1 p-6 overflow-auto bg-gray-50">
          {renderContent()}
        </main>
      </div>
    </div>
  );
}
