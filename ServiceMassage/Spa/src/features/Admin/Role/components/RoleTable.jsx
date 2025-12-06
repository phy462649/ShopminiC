// RoleTable.jsx
import { useQuery } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import { message } from "antd";
import RoleForm from "./RoleForm";

const initialRoles = [
  {
    id: 1,
    name: "admin",
    description: "Toàn quyền hệ thống",
    created_at: "2024-01-01T10:00:00",
    updated_at: "2024-01-01T10:00:00",
  },
  {
    id: 2,
    name: "staff",
    description: "Nhân viên bán hàng",
    created_at: "2024-01-05T11:00:00",
    updated_at: "2024-01-05T11:00:00",
  },
];

const fetchRoles = async () => initialRoles;

export default function RoleTable() {
  const {
    data = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["roles"],
    queryFn: fetchRoles,
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
    ? list.filter((r) => r.name.toLowerCase().includes(q.toLowerCase()))
    : list;

  const handleSave = (input) => {
    if (editing) {
      setList((prev) =>
        prev.map((r) =>
          r.id === editing.id
            ? { ...r, ...input, updated_at: new Date().toISOString() }
            : r
        )
      );
    } else {
      setList((prev) => [
        ...prev,
        {
          ...input,
          id: prev.length ? Math.max(...prev.map((r) => r.id)) + 1 : 1,
          created_at: new Date().toISOString(),
          updated_at: new Date().toISOString(),
        },
      ]);
    }

    setEditing(null);
    setFormOpen(false);
  };

  const handleDelete = () => {
    if (!selectedId) return message.error("Chọn vai trò để xoá");

    setList((prev) => prev.filter((r) => r.id !== selectedId));
    setSelectedId(null);
    message.success("Đã xoá vai trò!");
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
          placeholder="Tìm vai trò..."
          className="w-64 mr-auto pl-3 pr-3 py-2 border rounded-md focus:ring-2 focus:ring-pink-500"
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
              const item = list.find((r) => r.id === selectedId);
              if (!item) return message.warning("Chọn vai trò để sửa");
              setEditing(item);
              setFormOpen(true);
            }}
            disabled={!selectedId}
            className="px-4 py-2 bg-yellow-500 text-white rounded-md"
          >
            Sửa
          </button>

          <button
            onClick={handleDelete}
            disabled={!selectedId}
            className="px-4 py-2 bg-red-600 text-white rounded-md"
          >
            Xoá
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
              <th className="p-2 border">Tạo lúc</th>
              <th className="p-2 border">Cập nhật</th>
            </tr>
          </thead>

          <tbody>
            {filtered.map((r) => (
              <tr
                key={r.id}
                onClick={() => setSelectedId(r.id)}
                className={`cursor-pointer hover:bg-gray-50 ${
                  selectedId === r.id ? "bg-pink-100" : ""
                }`}
              >
                <td className="p-2 border text-center">{r.id}</td>
                <td className="p-2 border">{r.name}</td>
                <td className="p-2 border">{r.description || "-"}</td>
                <td className="p-2 border text-center">
                  {new Date(r.created_at).toLocaleDateString("vi-VN")}
                </td>
                <td className="p-2 border text-center">
                  {new Date(r.updated_at).toLocaleDateString("vi-VN")}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Form */}
      {formOpen && (
        <RoleForm
          initialData={
            editing || {
              name: "",
              description: "",
            }
          }
          onSave={handleSave}
          onClose={() => setFormOpen(false)}
        />
      )}
    </div>
  );
}
