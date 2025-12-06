import { useQuery } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import { message } from "antd";
import CategoryForm from "./CategoryForm";

const initialCategories = [
  {
    id: 1,
    name: "Dịch vụ tóc",
    description: "Tất cả dịch vụ cắt – uốn – nhuộm",
    status: 1,
    parent_id: null,
    created_at: "2024-01-01T10:00:00",
  },
  {
    id: 2,
    name: "Nhuộm tóc",
    description: "Các gói nhuộm màu",
    status: 1,
    parent_id: 1,
    created_at: "2024-01-03T11:00:00",
  },
];

const fetchCategories = async () => initialCategories;

export default function CategoryTable() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ["categories"],
    queryFn: fetchCategories,
  });

  const [categoryList, setCategoryList] = useState([]);
  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState(null);

  const emptyCategory = {
    name: "",
    description: "",
    status: 1,
    parent_id: null,
  };

  useEffect(() => {
    if (data) setCategoryList(data);
  }, [data]);

  const handleAdd = () => {
    setEditingCategory(null);
    setFormOpen(true);
  };

  const handleUpdate = () => {
    if (!selectedId) return message.warning("Chọn category để sửa");

    const category = categoryList.find((c) => c.id === selectedId);
    if (!category) return;

    setEditingCategory(category);
    setFormOpen(true);
  };

  const handleDelete = () => {
    if (!selectedId) return message.error("Chọn category để xoá");

    setCategoryList((prev) => prev.filter((c) => c.id !== selectedId));
    setSelectedId(null);
    message.success("Đã xoá category!");
  };

  const handleSave = (category) => {
    if (editingCategory) {
      // update
      setCategoryList((prev) =>
        prev.map((c) =>
          c.id === editingCategory.id ? { ...c, ...category } : c
        )
      );
    } else {
      // create
      setCategoryList((prev) => [
        ...prev,
        {
          ...category,
          id: prev.length ? Math.max(...prev.map((c) => c.id)) + 1 : 1,
          created_at: new Date().toISOString(),
        },
      ]);
    }

    setFormOpen(false);
    setEditingCategory(null);
  };

  if (isLoading) return <p className="p-4">Đang tải...</p>;
  if (isError) return <p className="p-4 text-red-600">Lỗi tải dữ liệu</p>;

  return (
    <div className="p-4">
      {/* Toolbar */}
      <div className="mb-4 flex items-center">
        <div className="relative w-64 mr-auto">
          <input
            type="text"
            placeholder="Tìm category..."
            className="w-full pl-10 pr-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
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

        <div className="flex mx-8">
          <button
            className="px-4 py-2 bg-green-500 text-white rounded-md"
            onClick={handleAdd}
          >
            Thêm
          </button>

          <button
            className="px-4 py-2 bg-yellow-500 text-white rounded-md mx-4"
            onClick={handleUpdate}
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
      </div>

      {/* Table */}
      <div className="overflow-x-auto">
        <table className="min-w-full border border-gray-300 text-base">
          <thead className="bg-gray-100">
            <tr>
              <th className="p-2 border">ID</th>
              <th className="p-2 border">Tên</th>
              <th className="p-2 border">Mô tả</th>
              <th className="p-2 border">Parent ID</th>
              <th className="p-2 border">Trạng thái</th>
              <th className="p-2 border">Ngày tạo</th>
            </tr>
          </thead>

          <tbody>
            {categoryList.map((c) => (
              <tr
                key={c.id}
                className={`cursor-pointer hover:bg-gray-50 ${
                  selectedId === c.id ? "bg-pink-100" : ""
                }`}
                onClick={() => setSelectedId(c.id)}
              >
                <td className="p-2 border text-center">{c.id}</td>
                <td className="p-2 border">{c.name}</td>
                <td className="p-2 border">{c.description || "-"}</td>
                <td className="p-2 border text-center">{c.parent_id ?? "-"}</td>
                <td className="p-2 border text-center">
                  {c.status ? "Active" : "Inactive"}
                </td>
                <td className="p-2 border text-center">
                  {new Date(c.created_at).toLocaleDateString("vi-VN")}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Form */}
      {formOpen && (
        <CategoryForm
          initialData={editingCategory || emptyCategory}
          onClose={() => setFormOpen(false)}
          onSave={handleSave}
          categories={categoryList}
        />
      )}
    </div>
  );
}
