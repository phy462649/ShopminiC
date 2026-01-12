import { lazy, Suspense } from "react";
import { createBrowserRouter, RouterProvider, Navigate } from "react-router-dom";
import ProtectedRoute from "../Component/auth/ProtectedRoute";

// Layouts
const AdminLayout = lazy(() => import("../Layouts/AdminLayout"));
const UserLayout = lazy(() => import("../Layouts/UserLayout"));

// Auth Components
const LoginForm = lazy(() => import("../features/Admin/LoginRegister/Component/LoginForm"));
const RegisterStep1 = lazy(() => import("../features/Admin/LoginRegister/Component/RegisterStep1"));
const RegisterStep2 = lazy(() => import("../features/Admin/LoginRegister/Component/RegisterStep2"));
const VerifyUser = lazy(() => import("../features/Admin/LoginRegister/Component/VerifyUser"));
const ForgotPassword = lazy(() => import("../features/Admin/LoginRegister/Component/ForgotPassword"));
const ResetPassword = lazy(() => import("../features/Admin/LoginRegister/Component/ResetPassword"));

// User Pages
const Home = lazy(() => import("../Component/pages/Home"));
const Services = lazy(() => import("../Component/pages/Services"));
const Products = lazy(() => import("../Component/pages/Products"));
const About = lazy(() => import("../Component/pages/About"));
const Contact = lazy(() => import("../Component/pages/Contact"));

// Dashboard Component
const Dashboard = lazy(() => import("../Component/pages/Dashboard"));

// Error Pages
const NotFound = lazy(() => import("../Component/pages/NotFound"));
const Unauthorized = lazy(() => import("../Component/pages/Unauthorized"));

// Admin Components - Lazy loaded
const PersonalTable = lazy(() => import("../features/Admin/Personal/components/PersonalTable"));
const BookingTable = lazy(() => import("../features/Admin/Bookings/components/BookingTable"));
const CategoryTable = lazy(() => import("../features/Admin/Categories/components/CategoryTable"));
const OrdersTable = lazy(() => import("../features/Admin/Orders/components/OrdersTable"));
const OrderItemTable = lazy(() => import("../features/Admin/OrderItem/components/OrderItemTable"));
const PaymentTable = lazy(() => import("../features/Admin/Payment/components/PaymentTable"));
const ProductTable = lazy(() => import("../features/Admin/Product/components/ProductTable"));
const RoomTable = lazy(() => import("../features/Admin/Room/components/RoomTable"));
const RoleTable = lazy(() => import("../features/Admin/Role/components/RoleTable"));
const ServiceTable = lazy(() => import("../features/Admin/Services/components/ServiceTable"));
const StaffTable = lazy(() => import("../features/Admin/Staff/components/StaffTable"));
const StaffScheduleTable = lazy(() => import("../features/Admin/Staffschedule/components/StaffScheduleTable"));

// Loading Component
function Loading() {
  return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-gray-900"></div>
    </div>
  );
}

// Router configuration
const router = createBrowserRouter([
  // User Routes with Layout
  {
    path: "/",
    element: (
      <Suspense fallback={<Loading />}>
        <UserLayout />
      </Suspense>
    ),
    children: [
      { index: true, element: <Suspense fallback={<Loading />}><Home /></Suspense> },
      { path: "services", element: <Suspense fallback={<Loading />}><Services /></Suspense> },
      { path: "products", element: <Suspense fallback={<Loading />}><Products /></Suspense> },
      { path: "about", element: <Suspense fallback={<Loading />}><About /></Suspense> },
      { path: "contact", element: <Suspense fallback={<Loading />}><Contact /></Suspense> },
    ],
  },
  // Auth Routes
  {
    path: "/login",
    element: <Suspense fallback={<Loading />}><LoginForm /></Suspense>,
  },
  {
    path: "/register",
    element: <Suspense fallback={<Loading />}><RegisterStep1 /></Suspense>,
  },
  {
    path: "/register-step1",
    element: <Navigate to="/register" replace />,
  },
  {
    path: "/register-step2",
    element: <Suspense fallback={<Loading />}><RegisterStep2 /></Suspense>,
  },
  {
    path: "/verify-user",
    element: <Suspense fallback={<Loading />}><VerifyUser /></Suspense>,
  },
  {
    path: "/forgot-password",
    element: <Suspense fallback={<Loading />}><ForgotPassword /></Suspense>,
  },
  {
    path: "/reset-password",
    element: <Suspense fallback={<Loading />}><ResetPassword /></Suspense>,
  },
  {
    path: "/unauthorized",
    element: <Suspense fallback={<Loading />}><Unauthorized /></Suspense>,
  },
  {
    path: "/404",
    element: <Suspense fallback={<Loading />}><NotFound /></Suspense>,
  },
  // Admin Routes
  {
    path: "/admin",
    element: (
      <ProtectedRoute requiredRoles={["Admin"]}>
        <Suspense fallback={<Loading />}>
          <AdminLayout />
        </Suspense>
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <Suspense fallback={<Loading />}><Dashboard /></Suspense> },
      { path: "dashboard", element: <Suspense fallback={<Loading />}><Dashboard /></Suspense> },
      { path: "Personal", element: <Suspense fallback={<Loading />}><PersonalTable /></Suspense> },
      { path: "Booking", element: <Suspense fallback={<Loading />}><BookingTable /></Suspense> },
      { path: "Category", element: <Suspense fallback={<Loading />}><CategoryTable /></Suspense> },
      { path: "Orders", element: <Suspense fallback={<Loading />}><OrdersTable /></Suspense> },
      { path: "OrderItem", element: <Suspense fallback={<Loading />}><OrderItemTable /></Suspense> },
      { path: "Payment", element: <Suspense fallback={<Loading />}><PaymentTable /></Suspense> },
      { path: "Product", element: <Suspense fallback={<Loading />}><ProductTable /></Suspense> },
      { path: "Room", element: <Suspense fallback={<Loading />}><RoomTable /></Suspense> },
      { path: "Role", element: <Suspense fallback={<Loading />}><RoleTable /></Suspense> },
      { path: "Service", element: <Suspense fallback={<Loading />}><ServiceTable /></Suspense> },
      { path: "Staff", element: <Suspense fallback={<Loading />}><StaffTable /></Suspense> },
      { path: "StaffSchedule", element: <Suspense fallback={<Loading />}><StaffScheduleTable /></Suspense> },
    ],
  },
  // Redirect unknown routes
  {
    path: "*",
    element: <Navigate to="/" replace />,
  },
]);

export default function Router() {
  return <RouterProvider router={router} />;
}
