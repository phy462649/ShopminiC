// ServiceForm.jsx
import { useState } from "react";

export default function ServiceForm({ initialData, onSave, onClose }) {
  const [input, setInput] = useState(initialData);

  const handle = (k, v) => setInput((p) => ({ ...p, [k]: v }));

  const handleSubmit = () => {
    if (!input.name.trim()) return alert("Tên dịch vụ không được trống");
    if (!input.duration_minutes || input.duration_minutes <= 0)
      return alert("Thời lượng phải > 0");
    if (input.price <= 0) return alert("Giá phải > 0");

    onSave(input);
  };

  return (
    <div
      className="fixed inset-0 bg-black/40 flex items-center justify-center p-4"
      role="dialog"
      aria-modal="true"
    >
      <div className="bg-white w-full max-w-md p-6 rounded-md shadow">
        <h2 className="text-xl font-semibold mb-4">
          {initialData?.id ? "Sửa dịch vụ" : "Thêm dịch vụ"}
        </h2>

        <label className="block mb-3">
          <span className="text-sm font-medium">Tên dịch vụ *</span>
          <input
            value={input.name}
            onChange={(e) => handle("name", e.target.value)}
            className="w-full mt-1 border rounded px-3 py-2"
            placeholder="Ví dụ: Gội đầu thư giãn"
          />
        </label>

        <label className="block mb-3">
          <span className="text-sm font-medium">Mô tả</span>
          <textarea
            value={input.description || ""}
            onChange={(e) => handle("description", e.target.value)}
            className="w-full mt-1 border rounded px-3 py-2 min-h-[80px]"
            placeholder="Mô tả dịch vụ..."
          />
        </label>

        <label className="block mb-3">
          <span className="text-sm font-medium">Thời lượng (phút) *</span>
          <input
            type="number"
            value={input.duration_minutes}
            onChange={(e) => handle("duration_minutes", Number(e.target.value))}
            className="w-full mt-1 border rounded px-3 py-2"
            placeholder="VD: 30"
            min={1}
          />
        </label>

        <label className="block mb-4">
          <span className="text-sm font-medium">Giá *</span>
          <input
            type="number"
            value={input.price}
            onChange={(e) => handle("price", Number(e.target.value))}
            className="w-full mt-1 border rounded px-3 py-2"
            placeholder="VD: 150000"
            min={1}
          />
        </label>

        <div className="flex justify-end gap-3 pt-2">
          <button onClick={onClose} className="px-4 py-2 rounded bg-gray-200">
            Hủy
          </button>
          <button
            onClick={handleSubmit}
            className="px-4 py-2 rounded bg-pink-600 text-white"
          >
            Lưu
          </button>
        </div>
      </div>
    </div>
  );
}
