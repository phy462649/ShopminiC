// RoleForm.jsx
import { useState } from "react";

export default function RoleForm({ initialData, onSave, onClose }) {
  const [input, setInput] = useState(initialData);

  const handleChange = (key, value) => {
    setInput((prev) => ({ ...prev, [key]: value }));
  };

  const handleSubmit = () => {
    if (!input.name.trim()) return alert("Tên vai trò không được trống");
    onSave(input);
  };

  return (
    <div
      className="fixed inset-0 bg-black/40 flex items-center justify-center p-4"
      aria-modal="true"
      role="dialog"
    >
      <div className="bg-white w-full max-w-md p-6 rounded-md shadow">
        <h2 className="text-xl font-semibold mb-4">
          {initialData?.id ? "Sửa vai trò" : "Thêm vai trò"}
        </h2>

        <label className="block mb-3">
          <span className="text-sm font-medium">Tên vai trò *</span>
          <input
            value={input.name}
            onChange={(e) => handleChange("name", e.target.value)}
            className="w-full mt-1 border rounded px-3 py-2"
            placeholder="VD: admin"
          />
        </label>

        <label className="block mb-4">
          <span className="text-sm font-medium">Mô tả</span>
          <textarea
            value={input.description || ""}
            onChange={(e) => handleChange("description", e.target.value)}
            className="w-full mt-1 border rounded px-3 py-2 min-h-[80px]"
            placeholder="Mô tả vai trò..."
          />
        </label>

        <div className="flex justify-end gap-3">
          <button onClick={onClose} className="px-4 py-2 rounded bg-gray-200">
            Huỷ
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
