import { useState } from "react";
import { message, Spin } from "antd";
import { useCategory } from "../../Categories/hooks/useCategory";

export default function ProductForm({ initialData, onClose, onSave, isLoading }) {
  const { data: categories = [] } = useCategory();
  const isEditing = !!initialData?.id;

  const [formData, setFormData] = useState(() => ({
    name: initialData?.name || "",
    description: initialData?.description || "",
    price: initialData?.price || 0,
    stock: initialData?.stock || 0,
    categoryId: initialData?.categoryId || initialData?.category_id || "",
    urlImage: initialData?.urlImage || "",
  }));

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "price" || name === "stock" || name === "categoryId" 
        ? Number(value) 
        : value,
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    if (!formData.name.trim()) {
      return message.warning("Please enter product name");
    }
    if (!formData.price || formData.price <= 0) {
      return message.warning("Please enter valid price");
    }

    onSave(formData);
  };

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-lg max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          <h2 className="text-xl font-bold text-gray-800 mb-4">
            {isEditing ? "Edit Product" : "Add New Product"}
          </h2>

          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">Name *</label>
              <input
                type="text"
                name="name"
                value={formData.name}
                onChange={handleChange}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                placeholder="Enter product name"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Description</label>
              <textarea
                name="description"
                value={formData.description}
                onChange={handleChange}
                rows={3}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                placeholder="Enter description"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Category</label>
              <select
                name="categoryId"
                value={formData.categoryId}
                onChange={handleChange}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
              >
                <option value="">-- Select Category --</option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1">Price *</label>
                <input
                  type="number"
                  name="price"
                  min="0"
                  value={formData.price}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Stock *</label>
                <input
                  type="number"
                  name="stock"
                  min="0"
                  value={formData.stock}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Image URL</label>
              <input
                type="text"
                name="urlImage"
                value={formData.urlImage}
                onChange={handleChange}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                placeholder="Enter image URL"
              />
              {formData.urlImage && (
                <img 
                  src={formData.urlImage} 
                  alt="Preview" 
                  className="mt-2 w-24 h-24 object-cover rounded border"
                />
              )}
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
