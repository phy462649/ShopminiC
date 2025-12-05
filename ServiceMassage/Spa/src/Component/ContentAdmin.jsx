import { useState } from "react";
import Sidebar from "./Sidebar";

import Dashboard from "./pages/Dashboard";
import Users from "./pages/Users";
import Settings from "./pages/Settings";
import Logout from "./pages/Logout";

export default function ContentAdmin() {
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
        return <div className="p-4">Not Found</div>;
    }
  };

  return (
    <div className="flex h-screen">
      <Sidebar
        items={[
          { id: "dashboard", label: "Dashboard" },
          { id: "users", label: "Users" },
          { id: "settings", label: "Settings" },
          { id: "logout", label: "Logout" },
        ]}
        active={page}
        onSelect={setPage}
      />
      <main className="flex-1 p-4 overflow-auto ml-64">{renderContent()}</main>
    </div>
  );
}
