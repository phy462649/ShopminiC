import { useState, useEffect, useMemo } from "react";

export default function OrderItemsForm({
  initialData = null,
  products = [],
  onClose,
  onSave,
}) {
  const [form, setForm] = useState({
    product_id: "",
    quantity: 1,
    price: 0,
  });

  useEffect(() => {
    if (initialData) {
      setForm({
        product_id: initialData.product_id,
        quantity: initialData.quantity,
        price: initialData.price,
      });
    }
  }, [initialData]);

  const total = useMemo(() => {
    const q = Number(form.quantity) || 0;
    const p = Number(form.price) || 0;
    return (q * p).toFixed(2);
  }, [form.quantity, form.price]);

  const handleChange = (key, val) =>
    setForm((prev) => ({ ...prev, [key]: val }));

  const handleSubmit = () => {
    if (!form.product_id) return alert("Chọn sản phẩm");
    if (form.quantity <= 0) return alert("Số lượng phải > 0");
    if (form.price <= 0) return alert("Giá phải > 0");

    onSave(form);
  };

  return (
    <div
      className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center p-4 z-50"
      role="dialog"
      aria-modal="true"
    >
      <div className="bg-white rounded-xl shadow-lg w-full max-w-md p-6">
        <h2 className="text-xl font-semibold mb-4">
          {initialData ? "Sửa Order Item" : "Thêm Order Item"}
        </h2>

        {/* Product */}
        <label className="block mb-3">
          <span className="font-medium">Sản phẩm</span>
          <select
            className="mt-1 w-full border px-3 py-2 rounded-md"
            value={form.product_id}
            onChange={(e) => handleChange("product_id", Number(e.target.value))}
          >
            <option value="">-- chọn sản phẩm --</option>
            {products.map((p) => (
              <option key={p.id} value={p.id}>
                {p.name}
              </option>
            ))}
          </select>
        </label>

        {/* Quantity */}
        <label className="block mb-3">
          <span className="font-medium">Số lượng</span>
          <input
            type="number"
            min={1}
            className="mt-1 w-full border px-3 py-2 rounded-md"
            value={form.quantity}
            onChange={(e) => handleChange("quantity", Number(e.target.value))}
          />
        </label>

        {/* Price */}
        <label className="block mb-3">
          <span className="font-medium">Giá</span>
          <input
            type="number"
            min={1}
            step="0.01"
            className="mt-1 w-full border px-3 py-2 rounded-md"
            value={form.price}
            onChange={(e) => handleChange("price", Number(e.target.value))}
          />
        </label>

        {/* Total */}
        <div className="mb-4 text-gray-700">
          <span className="font-medium">Tổng:</span> {total} ₫
        </div>

        {/* Actions */}
        <div className="flex justify-end gap-3 mt-4">
          <button className="px-4 py-2 rounded-md border" onClick={onClose}>
            Hủy
          </button>

          <button
            onClick={handleSubmit}
            className="px-4 py-2 bg-blue-600 text-white rounded-md"
          >
            Lưu
          </button>
        </div>
      </div>
    </div>
  );
}
