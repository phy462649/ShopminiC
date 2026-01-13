import { useState } from "react";
import { message, Spin } from "antd";

export default function PaymentForm({ initialData, onClose, onSave, isLoading }) {
  const isEditing = !!initialData?.id;

  const [formData, setFormData] = useState(() => ({
    payment_type: initialData?.payment_type || "booking",
    booking_id: initialData?.booking_id || "",
    order_id: initialData?.order_id || "",
    amount: initialData?.amount || "",
    method: initialData?.method || "cash",
    status: initialData?.status || "pending",
  }));

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    if (!formData.amount) {
      return message.warning("Please enter amount");
    }

    onSave({
      ...formData,
      amount: Number(formData.amount),
      booking_id: formData.booking_id ? Number(formData.booking_id) : null,
      order_id: formData.order_id ? Number(formData.order_id) : null,
    });
  };

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-lg max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          <h2 className="text-xl font-bold text-gray-800 mb-4">
            {isEditing ? "Edit Payment" : "Add New Payment"}
          </h2>

          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1">Payment Type</label>
                <select
                  name="payment_type"
                  value={formData.payment_type}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                >
                  <option value="booking">Booking</option>
                  <option value="order">Order</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Method</label>
                <select
                  name="method"
                  value={formData.method}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                >
                  <option value="cash">Cash</option>
                  <option value="card">Card</option>
                  <option value="momo">Momo</option>
                  <option value="bank_transfer">Bank Transfer</option>
                </select>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1">Booking ID</label>
                <input
                  type="number"
                  name="booking_id"
                  value={formData.booking_id}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                  placeholder="Enter booking ID"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Order ID</label>
                <input
                  type="number"
                  name="order_id"
                  value={formData.order_id}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                  placeholder="Enter order ID"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Amount *</label>
              <input
                type="number"
                name="amount"
                value={formData.amount}
                onChange={handleChange}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                placeholder="Enter amount"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Status</label>
              <select
                name="status"
                value={formData.status}
                onChange={handleChange}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
              >
                <option value="pending">Pending</option>
                <option value="completed">Completed</option>
                <option value="failed">Failed</option>
              </select>
            </div>

            <div className="flex justify-end gap-3 pt-4 border-t">
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 border rounded-md hover:bg-gray-100"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isLoading}
                className="px-4 py-2 bg-pink-500 text-white rounded-md hover:bg-pink-600 disabled:opacity-50 flex items-center gap-2"
              >
                {isLoading && <Spin size="small" />}
                {isEditing ? "Update" : "Create"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
