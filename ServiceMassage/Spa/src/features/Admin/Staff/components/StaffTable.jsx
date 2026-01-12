import { useState } from "react";
import { message } from "antd";
import { useStaff } from "../../../../../Hooks/useStaff";

export default function StaffTable() {
  const {
    data = [],
    isLoading,
    isError,
  } = useStaff();

  const [search, setSearch] = useState("");
  const [selectedId, setSelectedId] = useState(null);

  const filtered = data.filter((s) => {
    const q = search.toLowerCase();
    return (
      s.name.toLowerCase().includes(q) ||
      (s.email || "").toLowerCase().includes(q)
    );
  });

  const handleDelete = () => {
    if (!selectedId) return message.error("Chọn nhân viên để xoá");
    message.warning("Chức năng xóa nhân viên cần thực hiện trong trang Quản lý Người dùng");
  };

  const handleAdd = () => message.info("Vui lòng thêm nhân viên ở trang Quản lý Người dùng và gán quyền Staff");

  const handleEdit = () => {
    if (!selectedId) return message.warning("Chọn nhân viên để sửa");
    message.info("Vui lòng sửa thông tin ở trang Quản lý Người dùng");
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
          className="px-4 py-2 text-white bg-green-600 rounded-md opacity-50 cursor-not-allowed"
          onClick={handleAdd}
          title="Thực hiện ở trang Người dùng"
        >
          Thêm
        </button>

        <button
          className="px-4 py-2 text-white bg-yellow-500 rounded-md opacity-50 cursor-not-allowed"
          onClick={handleEdit}
          title="Thực hiện ở trang Người dùng"
        >
          Sửa
        </button>

        <button
          className="px-4 py-2 text-white bg-red-500 rounded-md opacity-50 cursor-not-allowed"
          onClick={handleDelete}
          title="Thực hiện ở trang Người dùng"
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
            {(filtered && filtered.length > 0) ? (
              filtered.map((s) => (
                <tr
                  key={s.id}
                  tabIndex={0}
                  aria-selected={selectedId === s.id}
                  onClick={() => setSelectedId(s.id)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter" || e.key === " ") setSelectedId(s.id);
                  }}
                  className={`cursor-pointer hover:bg-gray-50 ${selectedId === s.id ? "bg-pink-100" : ""
                    }`}
                >
                  <td className="p-2 border">{s.id}</td>
                  <td className="p-2 border">{s.name}</td>
                  <td className="p-2 border">{s.phone || "-"}</td>
                  <td className="p-2 border">{s.email || "-"}</td>
                  <td className="p-2 border">
                    {s.createdAt
                      ? new Date(s.createdAt).toLocaleString("vi-VN")
                      : "-"}
                  </td>
                  <td className="p-2 border">
                    {s.updatedAt
                      ? new Date(s.updatedAt).toLocaleString("vi-VN")
                      : "-"}
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="6" className="p-4 text-center text-gray-500">
                  Không có nhân viên nào
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
