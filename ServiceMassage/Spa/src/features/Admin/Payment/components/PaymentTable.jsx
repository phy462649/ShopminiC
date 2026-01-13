import { useState } from "react";
import { message, Tag, Modal } from "antd";
import PaymentForm from "./PaymentForm";
import {
  usePayment,
  useCreatePayment,
  useUpdatePayment,
  useDeletePayment,
} from "../hooks/usePayment";

const statusColors = {
  pending: "orange",
  completed: "green",
  failed: "red",
};

const methodLabels = {
  cash: "Cash",
  card: "Card",
  momo: "Momo",
  bank_transfer: "Bank Transfer",
};

export default function PaymentTable() {
  const { data: payments = [], isLoading, isError } = usePayment();
  const createMutation = useCreatePayment();
  const updateMutation = useUpdatePayment();
  const deleteMutation = useDeletePayment();

  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingPayment, setEditingPayment] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  const filteredPayments = payments.filter((p) =>
    [p.payment_type, p.method, p.status, String(p.booking_id || ''), String(p.order_id || '')]
      .filter(Boolean)
      .some((field) => String(field).toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const handleAdd = () => {
    setEditingPayment(null);
    setFormOpen(true);
  };

  const handleEdit = () => {
    if (!selectedId) return message.warning("Please select a payment to edit");
    const payment = payments.find((p) => p.id === selectedId);
    if (payment) {
      setEditingPayment(payment);
      setFormOpen(true);
    }
  };

  const handleDelete = () => {
    if (!selectedId) return message.warning("Please select a payment to delete");
    Modal.confirm({
      title: "Confirm Delete",
      content: "Are you sure you want to delete this payment?",
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
    if (editingPayment) {
      updateMutation.mutate(
        { id: editingPayment.id, data },
        { onSuccess: () => setFormOpen(false) }
      );
    } else {
      createMutation.mutate(data, {
        onSuccess: () => setFormOpen(false),
      });
    }
  };

  if (isLoading) return <div className="p-4 text-center">Loading...</div>;
  if (isError) return <div className="p-4 text-center text-red-500">Error loading data</div>;

  return (
    <div className="p-4">
      {/* Toolbar */}
      <div className="mb-4 flex flex-wrap items-center justify-between gap-4">
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
              <th className="p-3 text-center font-semibold">ID</th>
              <th className="p-3 text-center font-semibold">Type</th>
              <th className="p-3 text-center font-semibold">Reference</th>
              <th className="p-3 text-center font-semibold">Amount</th>
              <th className="p-3 text-center font-semibold">Method</th>
              <th className="p-3 text-center font-semibold">Status</th>
              <th className="p-3 text-center font-semibold">Payment Time</th>
            </tr>
          </thead>
          <tbody>
            {filteredPayments.length > 0 ? (
              filteredPayments.map((p) => (
                <tr
                  key={p.id}
                  onClick={() => setSelectedId(p.id)}
                  className={`cursor-pointer border-b hover:bg-gray-50 ${selectedId === p.id ? "bg-pink-100" : ""}`}
                >
                  <td className="p-3 text-center font-medium">{p.id}</td>
                  <td className="p-3 text-center capitalize">{p.payment_type}</td>
                  <td className="p-3 text-center">{p.booking_id || p.order_id || "-"}</td>
                  <td className="p-3 text-center font-semibold text-pink-600">
                    ${Number(p.amount).toLocaleString("en-US")}
                  </td>
                  <td className="p-3 text-center">{methodLabels[p.method] || p.method}</td>
                  <td className="p-3 text-center">
                    <Tag color={statusColors[p.status] || "default"}>
                      {typeof p.status === 'string' 
                        ? p.status.charAt(0).toUpperCase() + p.status.slice(1)
                        : p.status}
                    </Tag>
                  </td>
                  <td className="p-3 text-center">
                    {p.payment_time ? new Date(p.payment_time).toLocaleString("en-US") : "-"}
                  </td>
                </tr>
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
        <PaymentForm
          key={editingPayment?.id || 'new'}
          initialData={editingPayment}
          onClose={() => setFormOpen(false)}
          onSave={handleSave}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      )}
    </div>
  );
}
