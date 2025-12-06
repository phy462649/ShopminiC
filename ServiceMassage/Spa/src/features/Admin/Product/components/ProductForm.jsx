import { useState, useEffect, useRef } from "react";

export default function ProductForm({ initialData, onSave, onClose }) {
  const [form, setForm] = useState(initialData);
  const firstInputRef = useRef(null);

  useEffect(() => {
    if (firstInputRef.current) firstInputRef.current.focus();
  }, []);

  const handleChange = (e) => {
    const { name, value } = e.target;

    setForm((prev) => ({
      ...prev,
      [name]: name === "price" || name === "stock" ? Number(value) : value,
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!form.name.trim()) return alert("Tên sản phẩm không được để trống");
    if (form.price <= 0) return alert("Giá phải lớn hơn 0");

    onSave(form);
  };

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-md">
        <h2 className="text-xl font-semibold mb-4">
          {initialData.id ? "Sửa sản phẩm" : "Thêm sản phẩm"}
        </h2>

        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Name */}
          <div>
            <label className="block mb-1 font-medium">Tên sản phẩm</label>
            <input
              ref={firstInputRef}
              type="text"
              name="name"
              value={form.name}
              onChange={handleChange}
              className="w-full border rounded-md p-2"
              placeholder="Nhập tên..."
              required
            />
          </div>

          {/* Description */}
          <div>
            <label className="block mb-1 font-medium">Mô tả</label>
            <textarea
              name="description"
              value={form.description}
              onChange={handleChange}
              className="w-full border rounded-md p-2"
              rows="3"
              placeholder="Mô tả..."
            />
          </div>

          {/* Price */}
          <div>
            <label className="block mb-1 font-medium">Giá (VND)</label>
            <input
              type="number"
              name="price"
              value={form.price}
              onChange={handleChange}
              className="w-full border rounded-md p-2"
              min="0"
              required
            />
          </div>

          {/* Stock */}
          <div>
            <label className="block mb-1 font-medium">Tồn kho</label>
            <input
              type="number"
              name="stock"
              value={form.stock}
              onChange={handleChange}
              className="w-full border rounded-md p-2"
              min="0"
            />
          </div>

          {/* Actions */}
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
