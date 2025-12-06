import { useState } from "react";

export default function CategoryForm({
  initialData,
  categories,
  onSave,
  onClose,
}) {
  const [form, setForm] = useState({
    name: initialData?.name || "",
    description: initialData?.description || "",
    status: initialData?.status ?? 1,
    parent_id: initialData?.parent_id || "",
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!form.name.trim()) return alert("Tên danh mục không được để trống");

    onSave({
      ...form,
      parent_id: form.parent_id === "" ? null : Number(form.parent_id),
      status: Number(form.status),
    });
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4">
      <form
        onSubmit={handleSubmit}
        className="bg-white w-full max-w-lg p-6 rounded-xl shadow-xl space-y-4"
        aria-label="Category form"
      >
        <h2 className="text-xl font-semibold mb-2">
          {initialData ? "Chỉnh sửa danh mục" : "Thêm danh mục"}
        </h2>

        {/* Name */}
        <div>
          <label htmlFor="name" className="font-medium block mb-1">
            Tên danh mục *
          </label>
          <input
            id="name"
            name="name"
            value={form.name}
            onChange={handleChange}
            className="w-full border rounded-md px-3 py-2"
            required
          />
        </div>

        {/* Description */}
        <div>
          <label htmlFor="description" className="font-medium block mb-1">
            Mô tả
          </label>
          <textarea
            id="description"
            name="description"
            value={form.description}
            onChange={handleChange}
            className="w-full border rounded-md px-3 py-2"
            rows={3}
          />
        </div>

        {/* Parent category */}
        <div>
          <label htmlFor="parent_id" className="font-medium block mb-1">
            Danh mục cha
          </label>
          <select
            id="parent_id"
            name="parent_id"
            value={form.parent_id}
            onChange={handleChange}
            className="w-full border rounded-md px-3 py-2"
          >
            <option value="">-- Không --</option>
            {categories
              .filter((c) => !initialData || c.id !== initialData.id)
              .map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
          </select>
        </div>

        {/* Status */}
        <div>
          <label htmlFor="status" className="font-medium block mb-1">
            Trạng thái
          </label>
          <select
            id="status"
            name="status"
            value={form.status}
            onChange={handleChange}
            className="w-full border rounded-md px-3 py-2"
          >
            <option value={1}>Hoạt động</option>
            <option value={0}>Không hoạt động</option>
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
