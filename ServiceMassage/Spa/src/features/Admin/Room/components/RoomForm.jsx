import { useState, useEffect, useRef } from "react";

export default function RoomForm({ initialData, onSave, onClose }) {
  const [form, setForm] = useState(initialData);
  const firstInputRef = useRef(null);

  useEffect(() => {
    setForm(initialData);
  }, [initialData]);

  useEffect(() => {
    if (firstInputRef.current) firstInputRef.current.focus();
    const onKey = (e) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [onClose]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: name === "capacity" ? Number(value) : value,
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!form.name.trim()) return alert("Tên phòng không được để trống");
    if (!form.capacity || form.capacity < 1) return alert("Sức chứa phải >= 1");
    onSave(form);
  };

  return (
    <div
      className="fixed inset-0 bg-black/40 flex items-center justify-center z-50"
      role="dialog"
      aria-modal="true"
      aria-labelledby="room-form-title"
    >
      <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-md">
        <h2 id="room-form-title" className="text-xl font-semibold mb-4">
          {initialData.id ? "Sửa phòng" : "Thêm phòng"}
        </h2>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block mb-1 font-medium" htmlFor="room-name">
              Tên phòng
            </label>
            <input
              ref={firstInputRef}
              id="room-name"
              name="name"
              type="text"
              value={form.name}
              onChange={handleChange}
              className="w-full border rounded-md p-2"
              placeholder="Nhập tên phòng..."
              required
            />
          </div>

          <div>
            <label className="block mb-1 font-medium" htmlFor="room-desc">
              Mô tả
            </label>
            <textarea
              id="room-desc"
              name="description"
              value={form.description}
              onChange={handleChange}
              className="w-full border rounded-md p-2"
              rows="3"
              placeholder="Mô tả ngắn..."
            />
          </div>

          <div>
            <label className="block mb-1 font-medium" htmlFor="room-cap">
              Sức chứa
            </label>
            <input
              id="room-cap"
              name="capacity"
              type="number"
              min="1"
              value={form.capacity}
              onChange={handleChange}
              className="w-full border rounded-md p-2"
              required
            />
          </div>

          <div className="flex justify-end mt-4 space-x-3">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 border rounded-md"
            >
              Hủy
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-pink-600 text-white rounded-md"
            >
              Lưu
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
