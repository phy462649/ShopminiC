import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import PersonalForm from "./PersonalForm";
import { message, Modal } from "antd";
import { personService } from "../../../../services";

export default function PersonalTable() {
  const queryClient = useQueryClient();
  
  const { data, isLoading, isError } = useQuery({
    queryKey: ["personals"],
    queryFn: personService.getAll,
  });

  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingPersonal, setEditingPersonal] = useState(null);

  const createMutation = useMutation({
    mutationFn: personService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["personals"] });
      message.success("Thêm người dùng thành công!");
      setFormOpen(false);
      setEditingPersonal(null);
    },
    onError: (error) => {
      message.error(error?.message || "Lỗi khi thêm người dùng");
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }) => personService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["personals"] });
      message.success("Cập nhật người dùng thành công!");
      setFormOpen(false);
      setEditingPersonal(null);
    },
    onError: (error) => {
      message.error(error?.message || "Lỗi khi cập nhật người dùng");
    },
  });

  const deleteMutation = useMutation({
    mutationFn: personService.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["personals"] });
      message.success("Xóa người dùng thành công!");
      setSelectedId(null);
    },
    onError: (error) => {
      message.error(error?.message || "Lỗi khi xóa người dùng");
    },
  });

  const personalsList = Array.isArray(data) ? data : [];

  const handleAdd = () => {
    setEditingPersonal(null);
    setFormOpen(true);
  };

  const handleUpdate = () => {
    if (!selectedId) {
      return message.warning("Vui lòng chọn người dùng để cập nhật");
    }
    const personal = personalsList.find((c) => c.id === selectedId);
    if (!personal) return;
    setEditingPersonal(personal);
    setFormOpen(true);
  };

  const handleDelete = () => {
    if (!selectedId) {
      return message.warning("Vui lòng chọn người dùng để xóa");
    }
    Modal.confirm({
      title: "Xác nhận xóa",
      content: "Bạn có chắc chắn muốn xóa người dùng này?",
      okText: "Xóa",
      cancelText: "Hủy",
      okButtonProps: { danger: true },
      onOk: () => deleteMutation.mutate(selectedId),
    });
  };

  const handleCloseForm = () => {
    setFormOpen(false);
    setEditingPersonal(null);
  };

  const handleSave = (personalData) => {
    if (editingPersonal?.id) {
      updateMutation.mutate({ id: editingPersonal.id, data: personalData });
    } else {
      createMutation.mutate(personalData);
    }
  };

  const handleRowClick = (id) => {
    setSelectedId(selectedId === id ? null : id);
  };

  if (isLoading) return <p className="p-4">Đang tải...</p>;
  if (isError) return <p className="p-4 text-red-600">Lỗi tải dữ liệu</p>;

  return (
    <div className="p-4">
      <div className="mb-4 flex items-center">
        <div className="relative w-64 mr-auto">
          <input
            type="text"
            placeholder="Tìm kiếm người dùng..."
            className="w-full pl-10 pr-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
          />
          <svg className="w-5 h-5 absolute left-3 top-2.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-4.35-4.35m0 0A7.5 7.5 0 1010.5 3a7.5 7.5 0 006.15 13.65z" />
          </svg>
        </div>
        <div className="flex gap-2">
          <button className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600" onClick={handleAdd}>
            Thêm
          </button>
          <button 
            className={`px-4 py-2 rounded-md ${selectedId ? "bg-yellow-500 hover:bg-yellow-600 text-white" : "bg-gray-300 text-gray-500 cursor-not-allowed"}`} 
            onClick={handleUpdate}
            disabled={!selectedId}
          >
            Sửa
          </button>
          <button 
            className={`px-4 py-2 rounded-md ${selectedId ? "bg-red-500 hover:bg-red-600 text-white" : "bg-gray-300 text-gray-500 cursor-not-allowed"}`}
            onClick={handleDelete} 
            disabled={!selectedId || deleteMutation.isPending}
          >
            {deleteMutation.isPending ? "Đang xóa..." : "Xóa"}
          </button>
        </div>
      </div>

      <div className="overflow-x-auto">
        <table className="min-w-full border border-gray-300 text-sm">
          <thead className="bg-gray-100">
            <tr>
              <th className="p-2 border">ID</th>
              <th className="p-2 border">Tên</th>
              <th className="p-2 border">Username</th>
              <th className="p-2 border">Email</th>
              <th className="p-2 border">SĐT</th>
              <th className="p-2 border">Địa chỉ</th>
              <th className="p-2 border">Role</th>
              <th className="p-2 border">Xác thực</th>
              <th className="p-2 border">Ngày tạo</th>
            </tr>
          </thead>
          <tbody>
            {personalsList.map((c) => (
              <tr
                key={c.id}
                className={`cursor-pointer transition-colors duration-150 
                  ${selectedId === c.id 
                    ? "bg-pink-200 hover:bg-pink-300" 
                    : "hover:bg-gray-100"
                  }`}
                onClick={() => handleRowClick(c.id)}
              >
                <td className="p-2 border text-center">{c.id}</td>
                <td className="p-2 border">{c.name}</td>
                <td className="p-2 border">{c.username}</td>
                <td className="p-2 border">{c.email}</td>
                <td className="p-2 border text-center">{c.phone}</td>
                <td className="p-2 border">{c.address}</td>
                <td className="p-2 border text-center">
                  <span className={`px-2 py-1 rounded text-xs ${c.roleName === "ADMIN" ? "bg-red-100 text-red-700" : "bg-blue-100 text-blue-700"}`}>
                    {c.roleName}
                  </span>
                </td>
                <td className="p-2 border text-center">
                  <span className={`px-2 py-1 rounded text-xs ${c.statusVerify ? "bg-green-100 text-green-700" : "bg-yellow-100 text-yellow-700"}`}>
                    {c.statusVerify ? "Đã xác thực" : "Chưa"}
                  </span>
                </td>
                <td className="p-2 border text-center">
                  {new Date(c.createdAt).toLocaleDateString("vi-VN")}
                </td>
              </tr>
            ))}
            {personalsList.length === 0 && (
              <tr>
                <td colSpan={9} className="p-4 text-center text-gray-500">Không có dữ liệu</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {formOpen && (
        <PersonalForm
          initialData={editingPersonal}
          onClose={handleCloseForm}
          onSave={handleSave}
          loading={createMutation.isPending || updateMutation.isPending}
        />
      )}
    </div>
  );
}
