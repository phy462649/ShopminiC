import { useState } from "react";
import { message, Tag, Modal } from "antd";
import StaffScheduleForm from "./StaffScheduleForm";
import {
  useStaffSchedule,
  useCreateStaffSchedule,
  useUpdateStaffSchedule,
  useDeleteStaffSchedule,
} from "../hooks/useStaffschedule";

export default function StaffScheduleTable() {
  const { data: schedules = [], isLoading, isError, error } = useStaffSchedule();
  const createMutation = useCreateStaffSchedule();
  const updateMutation = useUpdateStaffSchedule();
  const deleteMutation = useDeleteStaffSchedule();

  // Debug log
  console.log('StaffSchedule Data:', schedules);
  console.log('StaffSchedule isLoading:', isLoading);
  console.log('StaffSchedule isError:', isError);
  console.log('StaffSchedule error:', error);

  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingSchedule, setEditingSchedule] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  const filteredSchedules = schedules.filter((s) =>
    [s.staffName, s.dayOfWeekName]
      .filter(Boolean)
      .some((field) => String(field).toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const handleAdd = () => {
    setEditingSchedule(null);
    setFormOpen(true);
  };

  const handleEdit = () => {
    if (!selectedId) return message.warning("Please select a schedule to edit");
    const schedule = schedules.find((s) => s.id === selectedId);
    if (schedule) {
      setEditingSchedule(schedule);
      setFormOpen(true);
    }
  };

  const handleDelete = () => {
    if (!selectedId) return message.warning("Please select a schedule to delete");
    Modal.confirm({
      title: "Confirm Delete",
      content: "Are you sure you want to delete this schedule?",
      okText: "Delete",
      cancelText: "Cancel",
      okButtonProps: { danger: true },
      onOk: () => {
        deleteMutation.mutate(selectedId, {
          onSuccess: () => setSelectedId(null),
        });
      },
    });
  };

  const handleSave = (data) => {
    if (editingSchedule) {
      updateMutation.mutate(
        { id: editingSchedule.id, data },
        { onSuccess: () => setFormOpen(false) }
      );
    } else {
      createMutation.mutate(data, {
        onSuccess: () => setFormOpen(false),
      });
    }
  };

  if (isLoading) return <div className="p-4 text-center">Loading...</div>;
  if (isError) return <div className="p-4 text-center text-red-500">Error loading data</div>;

  return (
    <div className="p-4">
      {/* Toolbar */}
      <div className="mb-4 flex flex-wrap items-center justify-between gap-4">
        <div className="relative min-w-[200px] max-w-xs">
          <input
            type="text"
            placeholder="Search..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
          />
          <svg className="w-5 h-5 absolute left-3 top-2.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-4.35-4.35m0 0A7.5 7.5 0 1010.5 3a7.5 7.5 0 006.15 13.65z" />
          </svg>
        </div>

        <div className="flex gap-4">
          <button onClick={handleAdd} className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600">
            Add
          </button>
          <button onClick={handleEdit} className="px-4 py-2 bg-yellow-500 text-white rounded-md hover:bg-yellow-600">
            Edit
          </button>
          <button onClick={handleDelete} className="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600">
            Delete
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="overflow-x-auto bg-white rounded-lg shadow">
        <table className="min-w-full text-sm">
          <thead className="bg-pink-50">
            <tr>
              <th className="p-3 text-center font-semibold">ID</th>
              <th className="p-3 text-center font-semibold">Staff</th>
              <th className="p-3 text-center font-semibold">Day</th>
              <th className="p-3 text-center font-semibold">Start Time</th>
              <th className="p-3 text-center font-semibold">End Time</th>
              <th className="p-3 text-center font-semibold">Status</th>
            </tr>
          </thead>
          <tbody>
            {filteredSchedules.length > 0 ? (
              filteredSchedules.map((s) => (
                <tr
                  key={s.id}
                  onClick={() => setSelectedId(s.id)}
                  className={`cursor-pointer border-b ${selectedId === s.id ? "bg-pink-100" : "hover:bg-gray-50"}`}
                >
                  <td className="p-3 text-center font-medium">{s.id}</td>
                  <td className="p-3 text-center">{s.staffName}</td>
                  <td className="p-3 text-center">{s.dayOfWeekName}</td>
                  <td className="p-3 text-center">{s.startTime}</td>
                  <td className="p-3 text-center">{s.endTime}</td>
                  <td className="p-3 text-center">
                    <Tag color={s.isWorking ? "green" : "red"}>
                      {s.isWorking ? "Working" : "Off"}
                    </Tag>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="6" className="p-4 text-center text-gray-500">
                  No data available
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Form Modal */}
      {formOpen && (
        <StaffScheduleForm
          key={editingSchedule?.id || 'new'}
          initialData={editingSchedule}
          onClose={() => setFormOpen(false)}
          onSave={handleSave}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      )}
    </div>
  );
}
