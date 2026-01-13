import { useState } from "react";
import { message, Tag, Modal } from "antd";
import OrderForm from "./OrderForm";
import {
  useOrder,
  useCreateOrder,
  useUpdateOrder,
  useDeleteOrder,
} from "../hooks/useOrder";

const statusMap = {
  0: { label: "Pending", color: "orange" },
  1: { label: "Confirmed", color: "blue" },
  2: { label: "Shipped", color: "cyan" },
  3: { label: "Completed", color: "green" },
  4: { label: "Cancelled", color: "red" },
};

export default function OrderTable() {
  const { data: orders = [], isLoading, isError } = useOrder();
  const createMutation = useCreateOrder();
  const updateMutation = useUpdateOrder();
  const deleteMutation = useDeleteOrder();

  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingOrder, setEditingOrder] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [expandedId, setExpandedId] = useState(null);

  const filteredOrders = orders.filter((o) => {
    if (statusFilter !== "" && o.status !== Number(statusFilter)) return false;
    
    return [o.customerName, String(o.id)]
      .filter(Boolean)
      .some((field) => field.toLowerCase().includes(searchTerm.toLowerCase()));
  });

  const handleAdd = () => {
    setEditingOrder(null);
    setFormOpen(true);
  };

  const handleEdit = () => {
    if (!selectedId) return message.warning("Please select an order to edit");
    const order = orders.find((o) => o.id === selectedId);
    if (order) {
      setEditingOrder(order);
      setFormOpen(true);
    }
  };

  const handleDelete = () => {
    if (!selectedId) return message.warning("Please select an order to delete");
    Modal.confirm({
      title: "Confirm Delete",
      content: "Are you sure you want to delete this order?",
      okText: "Delete",
      cancelText: "Cancel",
      okButtonProps: { danger: true },
      onOk: () => {
        deleteMutation.mutate(selectedId, {
          onSuccess: () => setSelectedId(null),
        });
      },
    });
  };

  const handleSave = (data) => {
    if (editingOrder) {
      updateMutation.mutate(
        { id: editingOrder.id, data },
        { onSuccess: () => setFormOpen(false) }
      );
    } else {
      createMutation.mutate(data, {
        onSuccess: () => setFormOpen(false),
      });
    }
  };

  const toggleExpand = (id) => {
    setExpandedId(expandedId === id ? null : id);
  };

  const getStatusInfo = (status) => statusMap[status] || { label: status, color: "default" };

  if (isLoading) return <div className="p-4 text-center">Loading...</div>;
  if (isError) return <div className="p-4 text-center text-red-500">Error loading data</div>;

  return (
    <div className="p-4">
      {/* Toolbar */}
      <div className="mb-4 flex flex-wrap items-center justify-between gap-4">
        <div className="flex items-center gap-4">
          <div className="relative min-w-[200px] max-w-xs">
            <input
              type="text"
              placeholder="Search..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
            />
            <svg className="w-5 h-5 absolute left-3 top-2.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-4.35-4.35m0 0A7.5 7.5 0 1010.5 3a7.5 7.5 0 006.15 13.65z" />
            </svg>
          </div>
          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            className="border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
          >
            <option value="">All Status</option>
            <option value="0">Pending</option>
            <option value="1">Confirmed</option>
            <option value="2">Shipped</option>
            <option value="3">Completed</option>
            <option value="4">Cancelled</option>
          </select>
        </div>

        <div className="flex gap-4">
          <button onClick={handleAdd} className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600">
            Add
          </button>
          <button onClick={handleEdit} className="px-4 py-2 bg-yellow-500 text-white rounded-md hover:bg-yellow-600">
            Edit
          </button>
          <button onClick={handleDelete} className="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600">
            Delete
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="overflow-x-auto bg-white rounded-lg shadow">
        <table className="min-w-full text-sm">
          <thead className="bg-pink-50">
            <tr>
              <th className="p-3 text-center font-semibold w-8"></th>
              <th className="p-3 text-center font-semibold">ID</th>
              <th className="p-3 text-center font-semibold">Customer</th>
              <th className="p-3 text-center font-semibold">Order Time</th>
              <th className="p-3 text-center font-semibold">Total</th>
              <th className="p-3 text-center font-semibold">Status</th>
              {/* <th className="p-3 text-center font-semibold">Created At</th> */}
            </tr>
          </thead>
          <tbody>
            {filteredOrders.length > 0 ? (
              filteredOrders.map((o) => (
                <>
                  <tr
                    key={o.id}
                    onClick={() => setSelectedId(o.id)}
                    className={`cursor-pointer border-b hover:bg-gray-50 ${selectedId === o.id ? "bg-pink-100" : ""}`}
                  >
                    <td className="p-3 text-center">
                      <button
                        onClick={(e) => { e.stopPropagation(); toggleExpand(o.id); }}
                        className="text-gray-500 hover:text-pink-500"
                      >
                        {expandedId === o.id ? "▼" : "▶"}
                      </button>
                    </td>
                    <td className="p-3 text-center font-medium">{o.id}</td>
                    <td className="p-3 text-center">{o.customerName}</td>
                    <td className="p-3 text-center">
                      {o.orderTime ? new Date(o.orderTime).toLocaleString("en-US") : "-"}
                    </td>
                    <td className="p-3 text-center font-semibold text-pink-600">
                      ${o.totalAmount?.toLocaleString("en-US")}
                    </td>
                    <td className="p-3 text-center">
                      <Tag color={getStatusInfo(o.status).color}>
                        {getStatusInfo(o.status).label}
                      </Tag>
                    </td>
                    <td className="p-3 text-center">
                      {o.createdAt ? new Date(o.createdAt).toLocaleDateString("en-US") : "-"}
                    </td>
                  </tr>
                  {/* Expanded Row - Items */}
                  {expandedId === o.id && o.items && o.items.length > 0 && (
                    <tr key={`${o.id}-items`} className="bg-gray-50">
                      <td colSpan="7" className="p-4">
                        <div className="ml-8">
                          <h4 className="font-semibold mb-2 text-gray-700">Order Items:</h4>
                          <table className="w-full text-sm border">
                            <thead className="bg-gray-100">
                              <tr>
                                <th className="p-2 text-center border">#</th>
                                <th className="p-2 text-center border">Product</th>
                                <th className="p-2 text-center border">Unit Price</th>
                                <th className="p-2 text-center border">Quantity</th>
                                <th className="p-2 text-center border">Subtotal</th>
                              </tr>
                            </thead>
                            <tbody>
                              {o.items.map((item, idx) => (
                                <tr key={item.id || idx} className="border-b">
                                  <td className="p-2 border text-center">{idx + 1}</td>
                                  <td className="p-2 border text-center font-medium">{item.productName}</td>
                                  <td className="p-2 border text-center">${item.price?.toLocaleString("en-US")}</td>
                                  <td className="p-2 border text-center">{item.quantity}</td>
                                  <td className="p-2 border text-center font-semibold text-pink-600">
                                    ${item.subtotal?.toLocaleString("en-US")}
                                  </td>
                                </tr>
                              ))}
                              <tr className="bg-pink-50 font-semibold">
                                <td colSpan="4" className="p-2 border text-center">Total:</td>
                                <td className="p-2 border text-center text-pink-600">
                                  ${o.totalAmount?.toLocaleString("en-US")}
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                      </td>
                    </tr>
                  )}
                </>
              ))
            ) : (
              <tr>
                <td colSpan="7" className="p-4 text-center text-gray-500">
                  No data available
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Form Modal */}
      {formOpen && (
        <OrderForm
          key={editingOrder?.id || 'new'}
          initialData={editingOrder}
          onClose={() => setFormOpen(false)}
          onSave={handleSave}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      )}
    </div>
  );
}
