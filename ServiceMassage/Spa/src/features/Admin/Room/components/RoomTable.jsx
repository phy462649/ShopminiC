import { useQuery } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import { message } from "antd";
import RoomForm from "./RoomForm";

const initialRooms = [
  {
    id: 1,
    name: "Phòng VIP 1",
    description: "Phòng rộng, có sofa",
    capacity: 4,
    created_at: "2024-01-01T10:00:00",
    updated_at: "2024-01-01T10:00:00",
  },
  {
    id: 2,
    name: "Phòng Gội Đầu",
    description: "Ghế gội đầu cao cấp",
    capacity: 2,
    created_at: "2024-01-05T12:00:00",
    updated_at: "2024-01-05T12:00:00",
  },
];

const fetchRooms = async () => {
  // giả lập fetch (same UX as ProductTable)
  return new Promise((res) => setTimeout(() => res(initialRooms), 200));
};

export default function RoomTable() {
  const {
    data = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["rooms"],
    queryFn: fetchRooms,
  });

  const [list, setList] = useState([]);
  const [selectedId, setSelectedId] = useState(null);
  const [q, setQ] = useState("");
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState(null);

  useEffect(() => setList(data), [data]);

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
    if (!selectedId) return message.error("Chọn phòng để xoá");
    setList((prev) => prev.filter((r) => r.id !== selectedId));
    setSelectedId(null);
    message.success("Đã xoá phòng!");
  };

  if (isLoading) return <p className="p-4">Đang tải...</p>;
  if (isError) return <p className="p-4 text-red-600">Lỗi tải dữ liệu</p>;

  return (
    <div className="p-4">
      {/* Toolbar */}
      <div className="mb-4 flex items-center">
        <div className="relative w-64 mr-auto">
          <input
            value={q}
            onChange={(e) => setQ(e.target.value)}
            placeholder="Tìm phòng..."
            className="w-full pl-10 pr-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
            aria-label="Tìm phòng"
          />
          <svg
            className="w-5 h-5 absolute left-3 top-2.5 text-gray-400"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              d="M21 21l-4.35-4.35m0 0A7.5 7.5 0 1010.5 3a7.5 7.5 0 006.15 13.65z"
            />
          </svg>
        </div>

        <div className="flex mx-8 space-x-3">
          <button
            onClick={() => {
              setEditing(null);
              setFormOpen(true);
            }}
            className="px-4 py-2 bg-green-600 text-white rounded-md"
            aria-label="Thêm phòng"
          >
            Thêm
          </button>

          <button
            onClick={() => {
              const item = list.find((r) => r.id === selectedId);
              if (!item) return message.warning("Chọn phòng để sửa");
              setEditing(item);
              setFormOpen(true);
            }}
            className="px-4 py-2 bg-yellow-500 text-white rounded-md"
            disabled={!selectedId}
            aria-disabled={!selectedId}
            aria-label="Sửa phòng"
          >
            Sửa
          </button>

          <button
            onClick={handleDelete}
            className="px-4 py-2 bg-red-600 text-white rounded-md"
            disabled={!selectedId}
            aria-disabled={!selectedId}
            aria-label="Xóa phòng"
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
              <th scope="col" className="p-2 border">
                ID
              </th>
              <th scope="col" className="p-2 border">
                Tên
              </th>
              <th scope="col" className="p-2 border">
                Mô tả
              </th>
              <th scope="col" className="p-2 border">
                Sức chứa
              </th>
              <th scope="col" className="p-2 border">
                Ngày tạo
              </th>
              <th scope="col" className="p-2 border">
                Cập nhật
              </th>
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
                role="row"
              >
                <td className="p-2 border text-center">{r.id}</td>
                <td className="p-2 border">{r.name}</td>
                <td className="p-2 border">{r.description || "-"}</td>
                <td className="p-2 border text-center">{r.capacity}</td>
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
        <RoomForm
          initialData={
            editing || {
              name: "",
              description: "",
              capacity: 1,
            }
          }
          onSave={handleSave}
          onClose={() => {
            setFormOpen(false);
            setEditing(null);
          }}
        />
      )}
    </div>
  );
}
