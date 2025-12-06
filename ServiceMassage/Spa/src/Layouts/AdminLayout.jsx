import { useState } from "react";
import HeaderAdmin from "../features/Admin/Settings/components/HeaderAdmin";
import Sidebar from "../features/Admin/Settings/components/Sidebar";
import CustomerForm from "../features/Admin/Custommers/components/CustomersTable";
import Booking from "../features/Admin/Bookings/components/BookingTable";
import Category from "../features/Admin/Categories/components/CategoryTable";
import Orders from "../features/Admin/Orders/components/OrdersTable";
import OrdersItem from "../features/Admin/OrderItem/components/OrderItemTable";
import Payment from "../features/Admin/Payment/components/PaymentTable";
import Product from "../features/Admin/Product/components/ProductTable";
import Room from "../features/Admin/Room/components/RoomTable";
import Role from "../features/Admin/Role/components/RoleTable";
import Service from "../features/Admin/Services/components/ServiceTable";
import Staff from "../features/Admin/Staff/components/StaffTable";
import StaffSchedule from "../features/Admin/Staffschedule/components/StaffScheduleTable";

export default function AdminLayout() {
  const [page, setPage] = useState("dashboard");

  const renderContent = () => {
    switch (page) {
      // case "dashboard":
      //   return <Dashboard />;
      case "Customers":
        return <CustomerForm />;
      case "Booking":
        return <Booking />;
      case "Category":
        return <Category />;
      case "Orders":
        return <Orders />;
      case "OrderItem":
        return <OrdersItem />;
      case "Payment":
        return <Payment />;
      case "Product":
        return <Product />;
      case "Room":
        return <Room />;
      case "Role":
        return <Role />;
      case "Service":
        return <Service />;
      case "Staff":
        return <Staff />;
      case "StaffSchedule":
        return <StaffSchedule />;
      default:
        return <div className="p-4">Not Found</div>;
    }
  };

  const sidebarItems = [
    { id: "logo", label: "", isLogo: true }, // <--- logo ở vị trí đầu

    { id: "dashboard", label: "Dashboard" },
    { id: "Customers", label: "Customers" },
    { id: "Booking", label: "Booking" },
    { id: "Category", label: "Category" },
    { id: "Orders", label: "Orders" },
    { id: "OrderItem", label: "OrderItem" },
    { id: "Payment", label: "Payment" },
    { id: "Room", label: "Room" },
    { id: "Staff", label: "Staff" },
    { id: "StaffSchedule", label: "StaffSchedule" },
  ];

  return (
    <div className="flex h-screen overflow-hidden">
      <Sidebar items={sidebarItems} active={page} onSelect={setPage} />
      <div className="flex-1 flex flex-col ml-64">
        <HeaderAdmin
          user={{ name: "Admin", avatarUrl: "" }}
          onSignOut={() => setPage("logout")}
        />
        <main className="flex-1 p-6 overflow-auto bg-gray-50">
          {renderContent()}
        </main>
      </div>
    </div>
  );
}
