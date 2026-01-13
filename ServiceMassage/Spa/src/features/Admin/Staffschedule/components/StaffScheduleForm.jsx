import { useState } from "react";
import { message, Spin } from "antd";
import { useStaff } from "../../Staff/hooks/useStaff";

const dayLabels = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

export default function StaffScheduleForm({ initialData, onClose, onSave, isLoading }) {
  const { data: staffList = [] } = useStaff();
  const isEditing = !!initialData?.id;

  const [formData, setFormData] = useState(() => ({
    staffId: initialData?.staffId || "",
    dayOfWeek: initialData?.dayOfWeek ?? 1,
    startTime: initialData?.startTime || "",
    endTime: initialData?.endTime || "",
    isWorking: initialData?.isWorking ?? true,
  }));

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    if (!formData.staffId) {
      return message.warning("Please select a staff");
    }
    if (!formData.startTime || !formData.endTime) {
      return message.warning("Please enter start and end time");
    }

    onSave({
      staffId: Number(formData.staffId),
      dayOfWeek: Number(formData.dayOfWeek),
      startTime: formData.startTime,
      endTime: formData.endTime,
      isWorking: formData.isWorking,
    });
  };

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-md max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          <h2 className="text-xl font-bold text-gray-800 mb-4">
            {isEditing ? "Edit Schedule" : "Add New Schedule"}
          </h2>

          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">Staff *</label>
              <select
                name="staffId"
                value={formData.staffId}
                onChange={handleChange}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
              >
                <option value="">-- Select Staff --</option>
                {staffList.map((s) => (
                  <option key={s.id} value={s.id}>
                    {s.name}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Day of Week *</label>
              <select
                name="dayOfWeek"
                value={formData.dayOfWeek}
                onChange={handleChange}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
              >
                {dayLabels.map((day, idx) => (
                  <option key={idx} value={idx}>
                    {day}
                  </option>
                ))}
              </select>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1">Start Time *</label>
                <input
                  type="time"
                  name="startTime"
                  value={formData.startTime}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">End Time *</label>
                <input
                  type="time"
                  name="endTime"
                  value={formData.endTime}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                />
              </div>
            </div>

            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                name="isWorking"
                id="isWorking"
                checked={formData.isWorking}
                onChange={handleChange}
                className="w-4 h-4 text-pink-500 focus:ring-pink-500"
              />
              <label htmlFor="isWorking" className="text-sm font-medium">
                Working
              </label>
            </div>

            <div className="flex justify-end gap-3 pt-4 border-t">
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 border rounded-md hover:bg-gray-100"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isLoading}
                className="px-4 py-2 bg-pink-500 text-white rounded-md hover:bg-pink-600 disabled:opacity-50 flex items-center gap-2"
              >
                {isLoading && <Spin size="small" />}
                {isEditing ? "Update" : "Create"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
