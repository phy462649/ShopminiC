// src/router/index.jsx
import React, { lazy, Suspense } from "react";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import AppLayout from "../layouts/AppLayout";
import AuthLayout from "../layouts/AuthLayout";
import RequireAuth from "../router/RequireAuth";
import routeConfig from "../config/routes";

// lazy load pages
const HomePage = lazy(() => import("../pages/Home"));
const NotFoundPage = lazy(() => import("../pages/NotFound"));
const LoginPage = lazy(() => import("../features/auth/pages/LoginPage"));
const ProductListPage = lazy(() =>
  import("../features/product/pages/ProductListPage")
);
const ProductDetailPage = lazy(() =>
  import("../features/product/pages/ProductDetailPage")
);
const UserProfilePage = lazy(() =>
  import("../features/user/pages/ProfilePage")
);

function Loading() {
  return <div>Loading...</div>;
}

const router = createBrowserRouter([
  {
    path: "/",
    element: <AppLayout />,
    errorElement: (
      <Suspense fallback={<Loading />}>
        <NotFoundPage />
      </Suspense>
    ),
    children: [
      {
        index: true,
        element: (
          <Suspense fallback={<Loading />}>
            <HomePage />
          </Suspense>
        ),
      },
      {
        path: routeConfig.products,
        element: (
          <Suspense fallback={<Loading />}>
            <ProductListPage />
          </Suspense>
        ),
      },
      {
        path: `${routeConfig.products}/:id`,
        element: (
          <Suspense fallback={<Loading />}>
            <ProductDetailPage />
          </Suspense>
        ),
      },
      {
        path: routeConfig.user.profile,
        element: (
          <RequireAuth>
            <Suspense fallback={<Loading />}>
              <UserProfilePage />
            </Suspense>
          </RequireAuth>
        ),
      },
    ],
  },
  {
    path: "/auth",
    element: <AuthLayout />,
    children: [
      {
        path: "login",
        element: (
          <Suspense fallback={<Loading />}>
            <LoginPage />
          </Suspense>
        ),
      },
    ],
  },
]);

export default function Router() {
  return <RouterProvider router={router} />;
}
