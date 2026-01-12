import { useState, useEffect } from "react";
import { message } from "antd";
import StaffScheduleForm from "./StaffScheduleForm";
import {
  useStaffSchedules,
  useCreateStaffSchedule,
  useUpdateStaffSchedule,
  useDeleteStaffSchedule
} from "../../../../../Hooks/useStaffSchedule";

const dayLabels = ["CN", "T2", "T3", "T4", "T5", "T6", "T7"];

export default function StaffScheduleTable() {
  const { data, isLoading, isError } = useStaffSchedules();
  const createMutation = useCreateStaffSchedule();
  const updateMutation = useUpdateStaffSchedule();
  const deleteMutation = useDeleteStaffSchedule();

  const [scheduleList, setScheduleList] = useState([]);
  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingSchedule, setEditingSchedule] = useState(null);

  const emptySchedule = {
    staff_id: "",
    day_of_week: "",
    start_time: "",
    end_time: "",
    is_working: 1,
  };

  useEffect(() => {
    if (data) setScheduleList(data);
  }, [data]);

  const handleAdd = () => {
    setEditingSchedule(null);
    setFormOpen(true);
  };

  const handleUpdate = () => {
    if (!selectedId) return message.warning("Chọn lịch để sửa");

    const schedule = scheduleList.find((s) => s.id === selectedId);
    if (!schedule) return;

    setEditingSchedule(schedule);
    setFormOpen(true);
  };

  const handleDelete = () => {
    if (!selectedId) return message.error("Chọn lịch để xoá");

    if (window.confirm("Bạn có chắc chắn muốn xóa lịch này?")) {
      deleteMutation.mutate(selectedId, {
        onSuccess: () => {
          setSelectedId(null);
        }
      });
    }
  };

  const handleCloseForm = () => setFormOpen(false);

  const handleSave = (newSchedule) => {
    if (editingSchedule) {
      // update
      updateMutation.mutate({
        id: editingSchedule.id,
        data: newSchedule
      }, {
        onSuccess: () => {
          setFormOpen(false);
          setEditingSchedule(null);
        }
      });
    } else {
      // add new
      createMutation.mutate(newSchedule, {
        onSuccess: () => {
          setFormOpen(false);
          setEditingSchedule(null);
        }
      });
    }
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
            placeholder="Tìm kiếm lịch làm việc..."
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
              <th className="p-2 border">Nhân viên</th>
              <th className="p-2 border">Thứ</th>
              <th className="p-2 border">Bắt đầu</th>
              <th className="p-2 border">Kết thúc</th>
              <th className="p-2 border">Trạng thái</th>
            </tr>
          </thead>

          <tbody>
            {(scheduleList && scheduleList.length > 0) ? (
              scheduleList.map((s) => (
                <tr
                  key={s.id}
                  className={`cursor-pointer hover:bg-gray-50 ${selectedId === s.id ? "bg-pink-100" : ""
                    }`}
                  onClick={() => setSelectedId(s.id)}
                >
                  <td className="p-2 border text-center">{s.id}</td>
                  <td className="p-2 border text-center">{s.staff_name}</td>
                  <td className="p-2 border text-center">
                    {dayLabels[s.day_of_week]}
                  </td>
                  <td className="p-2 border text-center">{s.start_time}</td>
                  <td className="p-2 border text-center">{s.end_time}</td>
                  <td className="p-2 border text-center">
                    {s.is_working ? "Làm việc" : "Nghỉ"}
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="6" className="p-4 text-center text-gray-500">
                  Không có dữ liệu
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Form Add / Edit */}
      {formOpen && (
        <StaffScheduleForm
          initialData={editingSchedule || emptySchedule}
          onClose={handleCloseForm}
          onSave={handleSave}
        />
      )}
    </div>
  );
}
