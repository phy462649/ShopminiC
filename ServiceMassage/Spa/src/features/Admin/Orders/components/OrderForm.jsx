import { useState } from "react";
import { message, Spin } from "antd";
import { usePersonal } from "../../Personal/hooks/usePersonal";
import { useProduct } from "../../Product/hooks/useProduct";

const statusOptions = [
  { value: 0, label: "Pending" },
  { value: 1, label: "Confirmed" },
  { value: 2, label: "Shipped" },
  { value: 3, label: "Completed" },
  { value: 4, label: "Cancelled" },
];

export default function OrderForm({ initialData, onClose, onSave, isLoading }) {
  const { data: customers = [] } = usePersonal();
  const { data: products = [] } = useProduct();

  const isEditing = !!initialData?.id;
  const isPending = initialData?.status === 0 || !isEditing;
  const canEditItems = !isEditing || isPending;

  const formatDateTimeLocal = (dateStr) => {
    if (!dateStr) return "";
    const date = new Date(dateStr);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  };

  const [formData, setFormData] = useState(() => ({
    customerId: initialData?.customerId || initialData?.customer_id || "",
    orderTime: initialData?.orderTime || initialData?.order_time 
      ? formatDateTimeLocal(initialData.orderTime || initialData.order_time) 
      : "",
    status: initialData?.status ?? 0,
    note: initialData?.note || "",
  }));

  const [selectedItems, setSelectedItems] = useState(() => {
    if (initialData?.items?.length > 0) {
      return initialData.items.map(item => ({
        productId: item.productId || item.product_id,
        productName: item.productName || item.product_name,
        price: item.price,
        quantity: item.quantity,
      }));
    }
    return [];
  });

  const [currentProductId, setCurrentProductId] = useState("");
  const [currentQuantity, setCurrentQuantity] = useState(1);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleAddItem = () => {
    if (!currentProductId) {
      return message.warning("Please select a product");
    }
    
    const product = products.find(p => p.id === Number(currentProductId));
    if (!product) return;

    const exists = selectedItems.find(item => item.productId === product.id);
    if (exists) {
      return message.warning("Product already added");
    }

    setSelectedItems(prev => [...prev, {
      productId: product.id,
      productName: product.name,
      price: product.price,
      quantity: currentQuantity,
    }]);

    setCurrentProductId("");
    setCurrentQuantity(1);
  };

  const handleRemoveItem = (productId) => {
    setSelectedItems(prev => prev.filter(item => item.productId !== productId));
  };

  const handleQuantityChange = (productId, quantity) => {
    if (quantity < 1) return;
    setSelectedItems(prev => prev.map(item => 
      item.productId === productId ? { ...item, quantity } : item
    ));
  };

  const totalAmount = selectedItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);

  const handleSubmit = (e) => {
    e.preventDefault();
    
    if (!formData.customerId) {
      return message.warning("Please select a customer");
    }
    if (!formData.orderTime) {
      return message.warning("Please select order time");
    }
    if (selectedItems.length === 0) {
      return message.warning("Please add at least one product");
    }

    onSave({
      ...formData,
      customerId: Number(formData.customerId),
      status: Number(formData.status),
      items: selectedItems.map(item => ({
        productId: item.productId,
        quantity: item.quantity,
      })),
    });
  };

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          <h2 className="text-xl font-bold text-gray-800 mb-4">
            {isEditing ? "Edit Order" : "Add New Order"}
          </h2>

          <form onSubmit={handleSubmit} className="space-y-4">
            {/* Customer */}
            <div>
              <label className="block text-sm font-medium mb-1">Customer *</label>
              {isEditing ? (
                <input
                  type="text"
                  value={initialData?.customerName || `Customer #${formData.customerId}`}
                  disabled
                  className="w-full border rounded-md px-3 py-2 bg-gray-100 cursor-not-allowed"
                />
              ) : (
                <select
                  name="customerId"
                  value={formData.customerId}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                >
                  <option value="">-- Select Customer --</option>
                  {customers.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.name} - {c.phone}
                    </option>
                  ))}
                </select>
              )}
            </div>

            {/* Order Time */}
            <div>
              <label className="block text-sm font-medium mb-1">Order Time *</label>
              <input
                type="datetime-local"
                name="orderTime"
                value={formData.orderTime}
                onChange={handleChange}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
              />
            </div>

            {/* Products */}
            <div>
              <label className="block text-sm font-medium mb-1">Products *</label>
              
              {canEditItems && (
                <div className="flex gap-2 mb-2">
                  <select
                    value={currentProductId}
                    onChange={(e) => setCurrentProductId(e.target.value)}
                    className="flex-1 border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                  >
                    <option value="">-- Select Product --</option>
                    {products.map((p) => (
                      <option key={p.id} value={p.id}>
                        {p.name} - ${p.price?.toLocaleString("en-US")}
                      </option>
                    ))}
                  </select>
                  <input
                    type="number"
                    min="1"
                    value={currentQuantity}
                    onChange={(e) => setCurrentQuantity(Number(e.target.value))}
                    className="w-20 border rounded-md px-3 py-2 text-center focus:ring-2 focus:ring-pink-500"
                  />
                  <button
                    type="button"
                    onClick={handleAddItem}
                    className="px-4 py-2 bg-pink-500 text-white rounded-md hover:bg-pink-600"
                  >
                    + Add
                  </button>
                </div>
              )}

              {selectedItems.length > 0 && (
                <div className="border rounded-md overflow-hidden">
                  <table className="w-full text-sm">
                    <thead className="bg-gray-100">
                      <tr>
                        <th className="text-left p-2">Product</th>
                        <th className="text-center p-2 w-24">Qty</th>
                        <th className="text-right p-2">Price</th>
                        <th className="text-right p-2">Subtotal</th>
                        {canEditItems && <th className="text-center p-2 w-16">Action</th>}
                      </tr>
                    </thead>
                    <tbody>
                      {selectedItems.map((item) => (
                        <tr key={item.productId} className="border-t">
                          <td className="p-2">{item.productName}</td>
                          <td className="p-2 text-center">
                            {canEditItems ? (
                              <input
                                type="number"
                                min="1"
                                value={item.quantity}
                                onChange={(e) => handleQuantityChange(item.productId, Number(e.target.value))}
                                className="w-16 border rounded px-2 py-1 text-center"
                              />
                            ) : (
                              item.quantity
                            )}
                          </td>
                          <td className="p-2 text-right">${item.price?.toLocaleString("en-US")}</td>
                          <td className="p-2 text-right font-medium text-pink-600">
                            ${(item.price * item.quantity)?.toLocaleString("en-US")}
                          </td>
                          {canEditItems && (
                            <td className="p-2 text-center">
                              <button
                                type="button"
                                onClick={() => handleRemoveItem(item.productId)}
                                className="text-red-500 hover:text-red-700"
                              >
                                ✕
                              </button>
                            </td>
                          )}
                        </tr>
                      ))}
                    </tbody>
                    <tfoot className="bg-pink-50">
                      <tr className="font-semibold">
                        <td colSpan="3" className="p-2 text-right">Total:</td>
                        <td className="p-2 text-right text-pink-600">${totalAmount?.toLocaleString("en-US")}</td>
                        {canEditItems && <td></td>}
                      </tr>
                    </tfoot>
                  </table>
                </div>
              )}
            </div>

            {/* Status */}
            <div>
              <label className="block text-sm font-medium mb-1">Status</label>
              {isEditing ? (
                <input
                  type="text"
                  value={statusOptions.find(s => s.value === formData.status)?.label || 'Unknown'}
                  disabled
                  className="w-full border rounded-md px-3 py-2 bg-gray-100 cursor-not-allowed"
                />
              ) : (
                <select
                  name="status"
                  value={formData.status}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                >
                  <option value={0}>Pending</option>
                </select>
              )}
            </div>

            {/* Note */}
            <div>
              <label className="block text-sm font-medium mb-1">Note</label>
              <textarea
                name="note"
                value={formData.note}
                onChange={handleChange}
                rows={2}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                placeholder="Additional notes..."
              />
            </div>

            {/* Actions */}
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
