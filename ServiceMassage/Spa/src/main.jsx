import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import "antd/dist/reset.css";

import App from "./App.jsx";

// 1️⃣ import React Query
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

// 2️⃣ tạo QueryClient
const queryClient = new QueryClient();

// 3️⃣ bọc App bằng QueryClientProvider
createRoot(document.getElementById("root")).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <App />
    </QueryClientProvider>
  </StrictMode>
);
