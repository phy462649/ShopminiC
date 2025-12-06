import { useState } from "react";

export default function OrdersForm({
  initialData,
  customers,
  onSave,
  onClose,
}) {
  const [form, setForm] = useState({
    customer_id: initialData?.customer_id || "",
    order_time: initialData?.order_time || "",
    status: initialData?.status || "pending",
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!form.customer_id) return alert("Vui lòng chọn khách hàng");
    if (!form.order_time) return alert("Vui lòng chọn thời gian");

    onSave({
      customer_id: Number(form.customer_id),
      order_time: form.order_time,
      status: form.status,
    });
  };

  return (
    <div
      className="fixed inset-0 bg-black/50 flex items-center justify-center p-4"
      role="dialog"
      aria-modal="true"
    >
      <form
        onSubmit={handleSubmit}
        className="bg-white w-full max-w-lg p-6 rounded-xl shadow-xl space-y-4"
      >
        <h2 className="text-xl font-semibold">
          {initialData ? "Chỉnh sửa đơn hàng" : "Tạo đơn hàng"}
        </h2>

        {/* Customer */}
        <div>
          <label className="block mb-1 font-medium">Khách hàng *</label>
          <select
            name="customer_id"
            value={form.customer_id}
            onChange={handleChange}
            className="w-full border rounded-md px-3 py-2"
            required
          >
            <option value="">-- Chọn khách hàng --</option>
            {customers.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
        </div>

        {/* Order Time */}
        <div>
          <label className="block mb-1 font-medium">Thời gian đặt *</label>
          <input
            type="datetime-local"
            name="order_time"
            value={form.order_time}
            onChange={handleChange}
            className="w-full border rounded-md px-3 py-2"
            required
          />
        </div>

        {/* Status */}
        <div>
          <label className="block mb-1 font-medium">Trạng thái</label>
          <select
            name="status"
            value={form.status}
            onChange={handleChange}
            className="w-full border rounded-md px-3 py-2"
          >
            <option value="pending">Đang chờ</option>
            <option value="confirmed">Đã xác nhận</option>
            <option value="shipped">Đã gửi hàng</option>
            <option value="completed">Hoàn thành</option>
            <option value="cancelled">Hủy</option>
          </select>
        </div>

        {/* Actions */}
        <div className="flex justify-end gap-3 pt-4">
          <button
            type="button"
            onClick={onClose}
            className="px-4 py-2 bg-gray-200 rounded-md hover:bg-gray-300"
          >
            Đóng
          </button>

          <button
            type="submit"
            className="px-4 py-2 bg-pink-600 text-white rounded-md hover:bg-pink-700"
          >
            Lưu
          </button>
        </div>
      </form>
    </div>
  );
}
