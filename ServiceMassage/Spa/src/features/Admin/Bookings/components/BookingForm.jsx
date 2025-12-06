import { useState } from "react";

export default function BookingForm({ initialData, onClose, onSave }) {
  const [formData, setFormData] = useState({
    customer_id: initialData.customer_id || "",
    staff_id: initialData.staff_id || "",
    room_id: initialData.room_id || "",
    start_time: initialData.start_time || "",
    end_time: initialData.end_time || "",
    status: initialData.status || "pending",
  });

  const handleChange = (e) => {
    const { name, value } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = () => {
    if (!formData.customer_id || !formData.staff_id || !formData.room_id) {
      alert("customer_id, staff_id, room_id là bắt buộc");
      return;
    }

    onSave(formData);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-20 flex items-center justify-center p-4">
      <div className="bg-white p-6 rounded-lg shadow-lg w-full max-w-lg">
        <h2 className="text-xl font-semibold mb-4">
          {initialData && initialData.id ? "Sửa Booking" : "Thêm Booking"}
        </h2>

        {/* Form Fields */}
        <div className="space-y-4">
          <div>
            <label className="block font-medium mb-1">Customer ID</label>
            <input
              type="number"
              name="customer_id"
              value={formData.customer_id}
              onChange={handleChange}
              className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
            />
          </div>

          <div>
            <label className="block font-medium mb-1">Staff ID</label>
            <input
              type="number"
              name="staff_id"
              value={formData.staff_id}
              onChange={handleChange}
              className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
            />
          </div>

          <div>
            <label className="block font-medium mb-1">Room ID</label>
            <input
              type="number"
              name="room_id"
              value={formData.room_id}
              onChange={handleChange}
              className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
            />
          </div>

          <div>
            <label className="block font-medium mb-1">Thời gian bắt đầu</label>
            <input
              type="datetime-local"
              name="start_time"
              value={formData.start_time}
              onChange={handleChange}
              className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
            />
          </div>

          <div>
            <label className="block font-medium mb-1">Thời gian kết thúc</label>
            <input
              type="datetime-local"
              name="end_time"
              value={formData.end_time}
              onChange={handleChange}
              className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
            />
          </div>

          <div>
            <label className="block font-medium mb-1">Trạng thái</label>
            <select
              name="status"
              value={formData.status}
              onChange={handleChange}
              className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
            >
              <option value="pending">pending</option>
              <option value="confirmed">confirmed</option>
              <option value="completed">completed</option>
              <option value="cancelled">cancelled</option>
            </select>
          </div>
        </div>

        {/* Actions */}
        <div className="mt-6 flex justify-end gap-3">
          <button
            onClick={onClose}
            className="px-4 py-2 rounded-md border hover:bg-gray-100"
          >
            Huỷ
          </button>

          <button
            onClick={handleSubmit}
            className="px-4 py-2 bg-pink-600 text-white rounded-md hover:bg-pink-700"
          >
            Lưu
          </button>
        </div>
      </div>
    </div>
  );
}
