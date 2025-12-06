import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { message } from "antd";

// Fake API (JSX version)
const fetchStaff = async () => {
  return [
    {
      id: 1,
      name: "Nguyễn Văn A",
      phone: "0987000001",
      email: "a@example.com",
      specialty: "Massage",
      created_at: "2024-01-01T10:00:00",
      updated_at: "2024-01-05T10:00:00",
    },
    {
      id: 2,
      name: "Lê Thị B",
      phone: "0987000002",
      email: "b@example.com",
      specialty: "Skin care",
      created_at: "2024-02-02T10:00:00",
      updated_at: "2024-02-03T10:00:00",
    },
    {
      id: 3,
      name: "Trần Văn C",
      phone: "0987000003",
      email: "c@example.com",
      specialty: "Nails",
      created_at: "2024-03-02T10:00:00",
      updated_at: "2024-03-03T10:00:00",
    },
  ];
};

export default function StaffTable() {
  const {
    data = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["staff"],
    queryFn: fetchStaff,
  });

  const [search, setSearch] = useState("");
  const [selectedId, setSelectedId] = useState(null);

  const filtered = data.filter((s) => {
    const q = search.toLowerCase();
    return (
      s.name.toLowerCase().includes(q) ||
      (s.email || "").toLowerCase().includes(q) ||
      (s.specialty || "").toLowerCase().includes(q)
    );
  });

  const handleDelete = () => {
    if (!selectedId) return message.error("Chọn nhân viên để xoá");
    message.success("Xóa giả lập (mock)");
  };

  const handleAdd = () => message.info("Thêm (mock)");
  const handleEdit = () => {
    if (!selectedId) return message.warning("Chọn nhân viên để sửa");
    message.info("Sửa (mock)");
  };

  if (isLoading) return <p className="p-4">Đang tải...</p>;
  if (isError) return <p className="p-4 text-red-600">Lỗi tải dữ liệu</p>;

  return (
    <div className="p-4 space-y-4">
      {/* Toolbar */}
      <div className="flex items-center gap-4">
        <div className="relative w-64">
          <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Tìm kiếm nhân viên..."
            className="w-full pl-10 pr-3 py-2 border rounded-md focus:ring-2 focus:ring-pink-500"
            aria-label="Tìm kiếm staff"
          />
          <span className="absolute left-3 top-2.5 text-gray-400">🔍</span>
        </div>

        <button
          className="px-4 py-2 text-white bg-green-600 rounded-md"
          onClick={handleAdd}
        >
          Thêm
        </button>

        <button
          className="px-4 py-2 text-white bg-yellow-500 rounded-md"
          onClick={handleEdit}
        >
          Sửa
        </button>

        <button
          className="px-4 py-2 text-white bg-red-500 rounded-md"
          onClick={handleDelete}
        >
          Xoá
        </button>
      </div>

      {/* Table */}
      <div className="overflow-x-auto">
        <table className="min-w-full border border-gray-300" role="grid">
          <thead className="bg-gray-100">
            <tr>
              {[
                "ID",
                "Tên",
                "Điện thoại",
                "Email",
                "Chuyên môn",
                "Tạo lúc",
                "Cập nhật",
              ].map((h) => (
                <th key={h} className="p-2 border text-left">
                  {h}
                </th>
              ))}
            </tr>
          </thead>

          <tbody>
            {filtered.map((s) => (
              <tr
                key={s.id}
                tabIndex={0}
                aria-selected={selectedId === s.id}
                onClick={() => setSelectedId(s.id)}
                onKeyDown={(e) => {
                  if (e.key === "Enter" || e.key === " ") setSelectedId(s.id);
                }}
                className={`cursor-pointer hover:bg-gray-50 ${
                  selectedId === s.id ? "bg-pink-100" : ""
                }`}
              >
                <td className="p-2 border">{s.id}</td>
                <td className="p-2 border">{s.name}</td>
                <td className="p-2 border">{s.phone || "-"}</td>
                <td className="p-2 border">{s.email || "-"}</td>
                <td className="p-2 border">{s.specialty || "-"}</td>
                <td className="p-2 border">
                  {s.created_at
                    ? new Date(s.created_at).toLocaleString("vi-VN")
                    : "-"}
                </td>
                <td className="p-2 border">
                  {s.updated_at
                    ? new Date(s.updated_at).toLocaleString("vi-VN")
                    : "-"}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
