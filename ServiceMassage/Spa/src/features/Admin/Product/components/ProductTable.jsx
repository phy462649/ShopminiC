import { useQuery } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import { message } from "antd";
import ProductForm from "./ProductForm";

const initialProducts = [
  {
    id: 1,
    name: "Dầu gội A",
    description: "Sạch gàu, thơm lâu",
    price: 120000,
    stock: 30,
    created_at: "2024-01-01T10:00:00",
    updated_at: "2024-01-01T10:00:00",
  },
  {
    id: 2,
    name: "Sáp vuốt tóc B",
    description: "Giữ nếp mạnh",
    price: 180000,
    stock: 15,
    created_at: "2024-01-03T09:00:00",
    updated_at: "2024-01-03T09:00:00",
  },
];

const fetchProducts = async () => initialProducts;

export default function ProductTable() {
  const {
    data = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["products"],
    queryFn: fetchProducts,
  });

  const [list, setList] = useState([]);
  const [selectedId, setSelectedId] = useState(null);
  const [q, setQ] = useState("");
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState(null);

  useEffect(() => {
    setList(data);
  }, [data]);

  const filtered = q
    ? list.filter((p) => p.name.toLowerCase().includes(q.toLowerCase()))
    : list;

  const handleSave = (input) => {
    if (editing) {
      // update
      setList((prev) =>
        prev.map((p) =>
          p.id === editing.id
            ? { ...p, ...input, updated_at: new Date().toISOString() }
            : p
        )
      );
    } else {
      // create
      setList((prev) => [
        ...prev,
        {
          ...input,
          id: prev.length ? Math.max(...prev.map((p) => p.id)) + 1 : 1,
          created_at: new Date().toISOString(),
          updated_at: new Date().toISOString(),
        },
      ]);
    }

    setEditing(null);
    setFormOpen(false);
  };

  const handleDelete = () => {
    if (!selectedId) return message.error("Chọn sản phẩm để xoá");

    setList((prev) => prev.filter((p) => p.id !== selectedId));
    setSelectedId(null);
    message.success("Đã xoá sản phẩm!");
  };

  if (isLoading) return <p className="p-4">Đang tải...</p>;
  if (isError) return <p className="p-4 text-red-600">Lỗi tải dữ liệu</p>;

  return (
    <div className="p-4">
      {/* Toolbar */}
      <div className="mb-4 flex items-center">
        <input
          value={q}
          onChange={(e) => setQ(e.target.value)}
          placeholder="Tìm sản phẩm..."
          className="w-64 mr-auto pl-3 pr-3 py-2 border rounded-md 
                     focus:ring-2 focus:ring-pink-500"
        />

        <div className="flex mx-8 space-x-3">
          <button
            onClick={() => {
              setEditing(null);
              setFormOpen(true);
            }}
            className="px-4 py-2 bg-green-600 text-white rounded-md"
          >
            Thêm
          </button>

          <button
            onClick={() => {
              const item = list.find((p) => p.id === selectedId);
              if (!item) return message.warning("Chọn sản phẩm để sửa");
              setEditing(item);
              setFormOpen(true);
            }}
            className="px-4 py-2 bg-yellow-500 text-white rounded-md"
            disabled={!selectedId}
          >
            Sửa
          </button>

          <button
            onClick={handleDelete}
            className="px-4 py-2 bg-red-600 text-white rounded-md"
            disabled={!selectedId}
          >
            Xóa
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="overflow-x-auto">
        <table className="min-w-full border text-base">
          <thead className="bg-gray-100">
            <tr>
              <th className="p-2 border">ID</th>
              <th className="p-2 border">Tên</th>
              <th className="p-2 border">Mô tả</th>
              <th className="p-2 border">Giá</th>
              <th className="p-2 border">Tồn kho</th>
              <th className="p-2 border">Ngày tạo</th>
              <th className="p-2 border">Cập nhật</th>
            </tr>
          </thead>

          <tbody>
            {filtered.map((p) => (
              <tr
                key={p.id}
                onClick={() => setSelectedId(p.id)}
                className={`cursor-pointer hover:bg-gray-50 ${
                  selectedId === p.id ? "bg-pink-100" : ""
                }`}
              >
                <td className="p-2 border text-center">{p.id}</td>
                <td className="p-2 border">{p.name}</td>
                <td className="p-2 border">{p.description || "-"}</td>
                <td className="p-2 border text-center">
                  {p.price.toLocaleString()} đ
                </td>
                <td className="p-2 border text-center">{p.stock}</td>
                <td className="p-2 border text-center">
                  {new Date(p.created_at).toLocaleDateString("vi-VN")}
                </td>
                <td className="p-2 border text-center">
                  {new Date(p.updated_at).toLocaleDateString("vi-VN")}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Form */}
      {formOpen && (
        <ProductForm
          initialData={
            editing || {
              name: "",
              description: "",
              price: 0,
              stock: 0,
            }
          }
          onSave={handleSave}
          onClose={() => setFormOpen(false)}
        />
      )}
    </div>
  );
}
