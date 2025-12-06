import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { message } from "antd";
import OrderItemsForm from "./OrderItemsForm";

const initialOrderItems = [
  { id: 1, order_id: 1, product_id: 1, quantity: 2, price: 50000 },
  { id: 2, order_id: 1, product_id: 2, quantity: 1, price: 120000 },
];

const initialProducts = [
  { id: 1, name: "Sản phẩm A" },
  { id: 2, name: "Sản phẩm B" },
];

const fetchItems = async () => initialOrderItems;
const fetchProducts = async () => initialProducts;

export default function OrderItems() {
  const { data: itemsData } = useQuery({
    queryKey: ["order-items"],
    queryFn: fetchItems,
  });

  const { data: products } = useQuery({
    queryKey: ["products-order-items"],
    queryFn: fetchProducts,
  });

  const [list, setList] = useState([]);
  const [selectedId, setSelectedId] = useState(null);
  const [search, setSearch] = useState("");
  const [formOpen, setFormOpen] = useState(false);
  const [editingItem, setEditingItem] = useState(null);

  useEffect(() => {
    if (itemsData) setList(itemsData);
  }, [itemsData]);

  const filtered = list.filter((i) => {
    const p = products?.find((x) => x.id === i.product_id)?.name || "";
    return (
      p.toLowerCase().includes(search.toLowerCase()) ||
      String(i.id).includes(search)
    );
  });

  const handleAdd = () => {
    setEditingItem(null);
    setFormOpen(true);
  };

  const handleEdit = () => {
    if (!selectedId) return message.warning("Chọn item để sửa");
    const found = list.find((x) => x.id === selectedId);
    setEditingItem(found);
    setFormOpen(true);
  };

  const handleDelete = () => {
    if (!selectedId) return message.error("Chọn item để xóa");
    setList((prev) => prev.filter((x) => x.id !== selectedId));
    setSelectedId(null);
    message.success("Đã xóa item");
  };

  const handleSave = (data) => {
    if (editingItem) {
      setList((prev) =>
        prev.map((x) => (x.id === editingItem.id ? { ...x, ...data } : x))
      );
    } else {
      setList((prev) => [
        ...prev,
        {
          ...data,
          id: prev.length ? Math.max(...prev.map((x) => x.id)) + 1 : 1,
        },
      ]);
    }

    setFormOpen(false);
    setEditingItem(null);
  };

  return (
    <div className="p-4">
      {/* Toolbar */}
      <div className="flex items-center mb-4">
        <div className="relative w-72 mr-auto">
          <input
            type="text"
            placeholder="Tìm kiếm item..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full pl-10 pr-3 py-2 border rounded-md focus:ring-2 focus:ring-pink-500"
          />
          <svg
            className="w-5 h-5 absolute left-3 top-2.5 text-gray-400"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              d="M21 21l-4.35-4.35m0 0A7.5 7.5 0 1010.5 3a7.5 7.5 0 006.15 13.65z"
            />
          </svg>
        </div>

        <button
          className="px-4 py-2 bg-green-500 text-white rounded-md"
          onClick={handleAdd}
        >
          Thêm
        </button>

        <button
          className="px-4 py-2 bg-yellow-500 text-white rounded-md mx-4"
          onClick={handleEdit}
        >
          Sửa
        </button>

        <button
          className="px-4 py-2 bg-red-500 text-white rounded-md"
          onClick={handleDelete}
        >
          Xóa
        </button>
      </div>

      {/* Table */}
      <div className="overflow-x-auto">
        <table className="min-w-full border border-gray-300 text-base">
          <thead className="bg-gray-100">
            <tr>
              <th className="p-2 border">ID</th>
              <th className="p-2 border">Sản phẩm</th>
              <th className="p-2 border">Số lượng</th>
              <th className="p-2 border">Giá</th>
              <th className="p-2 border">Tổng</th>
            </tr>
          </thead>

          <tbody>
            {filtered.map((item) => {
              const p = products?.find((x) => x.id === item.product_id);

              return (
                <tr
                  key={item.id}
                  className={`cursor-pointer hover:bg-gray-50 ${
                    selectedId === item.id ? "bg-pink-100" : ""
                  }`}
                  onClick={() => setSelectedId(item.id)}
                >
                  <td className="p-2 border text-center">{item.id}</td>
                  <td className="p-2 border text-center">{p?.name || "N/A"}</td>
                  <td className="p-2 border text-center">{item.quantity}</td>
                  <td className="p-2 border text-center">
                    {item.price.toLocaleString()} ₫
                  </td>
                  <td className="p-2 border text-center">
                    {(item.price * item.quantity).toLocaleString()} ₫
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      {/* Form */}
      {formOpen && (
        <OrderItemsForm
          initialData={editingItem}
          products={products || []}
          onClose={() => setFormOpen(false)}
          onSave={handleSave}
        />
      )}
    </div>
  );
}
