import { useState } from "react";
import { useMutation } from "@tanstack/react-query";

// mock tạo payment
async function savePayment(payload) {
  return new Promise((resolve) => {
    setTimeout(() => resolve({ success: true, data: payload }), 600);
  });
}

export default function PaymentForm({ onSuccess }) {
  const [form, setForm] = useState({
    payment_type: "booking",
    reference_id: "",
    amount: "",
    method: "cash",
    status: "pending",
    created_by: "",
  });

  const { mutate, isPending, isError } = useMutation({
    mutationFn: savePayment,
    onSuccess: (res) => {
      onSuccess?.(res.data);
    },
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    mutate(form);
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="p-4 space-y-4 max-w-md bg-white rounded-lg border"
    >
      <h2 className="text-xl font-semibold">Thêm thanh toán</h2>

      {isError && (
        <p className="text-red-600 text-sm">Có lỗi xảy ra, thử lại sau.</p>
      )}

      <div>
        <label htmlFor="payment_type" className="block text-sm mb-1">
          Loại thanh toán
        </label>
        <select
          id="payment_type"
          name="payment_type"
          value={form.payment_type}
          onChange={handleChange}
          className="w-full border p-2 rounded"
        >
          <option value="booking">Booking</option>
          <option value="order">Order</option>
        </select>
      </div>

      <div>
        <label htmlFor="reference_id" className="block text-sm mb-1">
          Reference ID
        </label>
        <input
          id="reference_id"
          name="reference_id"
          type="number"
          required
          value={form.reference_id}
          onChange={handleChange}
          className="w-full border p-2 rounded"
        />
      </div>

      <div>
        <label htmlFor="amount" className="block text-sm mb-1">
          Số tiền
        </label>
        <input
          id="amount"
          name="amount"
          type="number"
          required
          min={1}
          value={form.amount}
          onChange={handleChange}
          className="w-full border p-2 rounded"
        />
      </div>

      <div>
        <label htmlFor="method" className="block text-sm mb-1">
          Phương thức
        </label>
        <select
          id="method"
          name="method"
          value={form.method}
          onChange={handleChange}
          className="w-full border p-2 rounded"
        >
          <option value="cash">Tiền mặt</option>
          <option value="card">Thẻ</option>
          <option value="bank_transfer">Chuyển khoản</option>
          <option value="momo">Momo</option>
          <option value="zalopay">ZaloPay</option>
        </select>
      </div>

      <div>
        <label htmlFor="status" className="block text-sm mb-1">
          Trạng thái
        </label>
        <select
          id="status"
          name="status"
          value={form.status}
          onChange={handleChange}
          className="w-full border p-2 rounded"
        >
          <option value="pending">Đang xử lý</option>
          <option value="completed">Hoàn thành</option>
          <option value="failed">Thất bại</option>
        </select>
      </div>

      <div>
        <label htmlFor="created_by" className="block text-sm mb-1">
          Người tạo
        </label>
        <input
          id="created_by"
          name="created_by"
          type="text"
          value={form.created_by}
          onChange={handleChange}
          className="w-full border p-2 rounded"
          required
        />
      </div>

      <button
        type="submit"
        disabled={isPending}
        className="w-full py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
      >
        {isPending ? "Đang lưu..." : "Lưu thanh toán"}
      </button>
    </form>
  );
}
