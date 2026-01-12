import { Outlet, useNavigate, useLocation } from "react-router-dom";
import HeaderAdmin from "../features/Admin/Settings/components/HeaderAdmin";
import Sidebar from "../features/Admin/Settings/components/Sidebar";

export default function AdminLayout() {
  const navigate = useNavigate();
  const location = useLocation();

  // Xác định trang active dựa trên pathname
  const getActivePage = () => {
    const path = location.pathname.replace('/admin/', '');
    return path || 'dashboard';
  };

  const activePage = getActivePage();

  const sidebarItems = [
    { id: "logo", label: "", isLogo: true }, // <--- logo ở vị trí đầu

    { id: "dashboard", label: "Dashboard" },
    { id: "Personal", label: "Personal" },
    { id: "Booking", label: "Booking" },
    { id: "Category", label: "Category" },
    { id: "Orders", label: "Orders" },
    { id: "OrderItem", label: "OrderItem" },
    { id: "Payment", label: "Payment" },
    { id: "Room", label: "Room" },
    { id: "Staff", label: "Staff" },
    { id: "StaffSchedule", label: "StaffSchedule" },
  ];

  const handleSidebarSelect = (pageId) => {
    navigate(`/admin/${pageId}`);
  };

  return (
    <div className="flex h-screen overflow-hidden">
      <Sidebar items={sidebarItems} active={activePage} onSelect={handleSidebarSelect} />
      <div className="flex-1 flex flex-col ml-64">
        <HeaderAdmin
          user={{ name: "Admin", avatarUrl: "" }}
          onSignOut={() => navigate("/login")}
        />
        <main className="flex-1 p-6 overflow-auto bg-gray-50">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
