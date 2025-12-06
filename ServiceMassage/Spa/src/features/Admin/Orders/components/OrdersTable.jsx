import { useState, useEffect, useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import OrdersForm from "./OrdersForm";
import { message } from "antd";

const initialOrders = [
  {
    id: 1,
    customer_id: 1,
    order_time: "2024-01-10T10:00:00",
    status: "pending",
    created_at: "2024-01-10T10:00:00",
  },
  {
    id: 2,
    customer_id: 2,
    order_time: "2024-01-11T11:00:00",
    status: "confirmed",
    created_at: "2024-01-11T11:00:00",
  },
];

const initialCustomers = [
  { id: 1, name: "Nguyễn Văn A" },
  { id: 2, name: "Trần Thị B" },
];

const fetchOrders = async () => initialOrders;
const fetchCustomers = async () => initialCustomers;

export default function Orders() {
  const { data: ordersData, isLoading } = useQuery({
    queryKey: ["orders"],
    queryFn: fetchOrders,
  });

  const { data: customers } = useQuery({
    queryKey: ["customers-orders"],
    queryFn: fetchCustomers,
  });

  const [list, setList] = useState([]);
  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingOrder, setEditingOrder] = useState(null);

  // search + filter state
  const [q, setQ] = useState("");
  const [statusFilter, setStatusFilter] = useState(""); // "" = all

  useEffect(() => {
    if (ordersData) setList(ordersData);
  }, [ordersData]);

  const handleAdd = () => {
    setEditingOrder(null);
    setFormOpen(true);
  };

  const handleEdit = () => {
    if (!selectedId) return message.warning("Chọn order để sửa");
    const found = list.find((o) => o.id === selectedId);
    setEditingOrder(found);
    setFormOpen(true);
  };

  const handleDelete = () => {
    if (!selectedId) return message.error("Chọn order để xóa");
    setList((prev) => prev.filter((o) => o.id !== selectedId));
    setSelectedId(null);
    message.success("Đã xóa đơn hàng");
  };

  const handleSave = (data) => {
    if (editingOrder) {
      // update
      setList((prev) =>
        prev.map((o) => (o.id === editingOrder.id ? { ...o, ...data } : o))
      );
    } else {
      // add
      setList((prev) => [
        ...prev,
        {
          ...data,
          id: prev.length ? Math.max(...prev.map((o) => o.id)) + 1 : 1,
          created_at: new Date().toISOString(),
        },
      ]);
    }

    setFormOpen(false);
    setEditingOrder(null);
  };

  // Derived filtered list (memoized)
  const filtered = useMemo(() => {
    const term = q.trim().toLowerCase();
    return list.filter((o) => {
      // status filter
      if (statusFilter && o.status !== statusFilter) return false;

      // search by customer name or id or status or order_time
      if (!term) return true;

      const customer = customers?.find((c) => c.id === o.customer_id);
      const customerName = customer?.name?.toLowerCase() || "";
      const idStr = String(o.id);
      const statusStr = String(o.status).toLowerCase();
      const timeStr = new Date(o.order_time)
        .toLocaleString("vi-VN")
        .toLowerCase();

      return (
        customerName.includes(term) ||
        idStr.includes(term) ||
        statusStr.includes(term) ||
        timeStr.includes(term)
      );
    });
  }, [list, q, statusFilter, customers]);

  if (isLoading) return <p className="p-4">Đang tải...</p>;

  return (
    <div className="p-4">
      {/* Toolbar: search + filters + actions */}
      <div className="mb-4 flex flex-col md:flex-row md:items-center gap-4">
        <div className="flex items-center gap-2 w-full md:w-1/2">
          <label htmlFor="orders-search" className="sr-only">
            Tìm kiếm đơn hàng
          </label>
          <div className="relative flex-1">
            <input
              id="orders-search"
              type="search"
              value={q}
              onChange={(e) => setQ(e.target.value)}
              placeholder="Tìm theo tên khách, ID, trạng thái hoặc thời gian..."
              className="w-full pl-10 pr-10 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
              aria-label="Tìm kiếm đơn hàng"
            />
            <svg
              className="w-5 h-5 absolute left-3 top-2.5 text-gray-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M21 21l-4.35-4.35m0 0A7.5 7.5 0 1010.5 3a7.5 7.5 0 006.15 13.65z"
              />
            </svg>

            {q && (
              <button
                aria-label="Clear search"
                onClick={() => setQ("")}
                className="absolute right-3 top-2.5 w-6 h-6 rounded-full flex items-center justify-center hover:bg-gray-100"
              >
                ×
              </button>
            )}
          </div>

          <label htmlFor="status-filter" className="sr-only">
            Lọc trạng thái
          </label>
          <select
            id="status-filter"
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            className="border rounded-md px-3 py-2"
            aria-label="Lọc theo trạng thái"
          >
            <option value="">Tất cả trạng thái</option>
            <option value="pending">pending</option>
            <option value="confirmed">confirmed</option>
            <option value="shipped">shipped</option>
            <option value="completed">completed</option>
            <option value="cancelled">cancelled</option>
          </select>
        </div>

        <div className="flex items-center gap-3 ml-auto">
          <button
            className="px-4 py-2 bg-green-500 text-white rounded-md"
            onClick={handleAdd}
          >
            Thêm
          </button>
          <button
            className="px-4 py-2 bg-yellow-500 text-white rounded-md"
            onClick={handleEdit}
            aria-disabled={!selectedId}
            disabled={!selectedId}
          >
            Sửa
          </button>
          <button
            className="px-4 py-2 bg-red-500 text-white rounded-md"
            onClick={handleDelete}
            aria-disabled={!selectedId}
            disabled={!selectedId}
          >
            Xóa
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="overflow-x-auto">
        <table className="min-w-full border border-gray-300 text-base">
          <thead className="bg-gray-100">
            <tr>
              <th className="p-2 border text-left">ID</th>
              <th className="p-2 border text-left">Khách hàng</th>
              <th className="p-2 border text-left">Thời gian đặt</th>
              <th className="p-2 border text-left">Trạng thái</th>
            </tr>
          </thead>

          <tbody>
            {filtered.length === 0 ? (
              <tr>
                <td colSpan="4" className="p-6 text-center text-gray-500">
                  Không tìm thấy đơn hàng.
                </td>
              </tr>
            ) : (
              filtered.map((o) => {
                const customerName =
                  customers?.find((c) => c.id === o.customer_id)?.name || "N/A";
                return (
                  <tr
                    key={o.id}
                    className={`cursor-pointer hover:bg-gray-50 ${
                      selectedId === o.id ? "bg-pink-100" : ""
                    }`}
                    onClick={() => setSelectedId(o.id)}
                    role="button"
                    tabIndex={0}
                    onKeyDown={(e) => {
                      if (e.key === "Enter" || e.key === " ")
                        setSelectedId(o.id);
                    }}
                    aria-pressed={selectedId === o.id}
                  >
                    <td className="p-2 border text-center">{o.id}</td>
                    <td className="p-2 border">{customerName}</td>
                    <td className="p-2 border">
                      {new Date(o.order_time).toLocaleString("vi-VN")}
                    </td>
                    <td className="p-2 border text-center">{o.status}</td>
                  </tr>
                );
              })
            )}
          </tbody>
        </table>
      </div>

      {/* Form */}
      {formOpen && (
        <OrdersForm
          initialData={editingOrder}
          customers={customers || []}
          onClose={() => setFormOpen(false)}
          onSave={handleSave}
        />
      )}
    </div>
  );
}
