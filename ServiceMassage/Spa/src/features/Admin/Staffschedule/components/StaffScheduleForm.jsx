import { useState } from "react";

export default function StaffScheduleForm({
  staffOptions = [],
  onSubmit,
  submitting = false,
  error = null,
}) {
  const [form, setForm] = useState({
    staff_id: "",
    day_of_week: "1",
    start_time: "",
    end_time: "",
    is_working: true,
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm((f) => ({
      ...f,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!form.staff_id) return alert("Chọn nhân viên");
    if (!form.start_time || !form.end_time)
      return alert("Nhập đầy đủ thời gian");

    onSubmit?.(form);
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="space-y-4 p-4 bg-white rounded-xl shadow"
    >
      <h2 className="text-lg font-semibold">Thêm lịch làm việc</h2>

      {error && <p className="text-red-600">{error}</p>}

      {/* STAFF */}
      <label className="block">
        <span className="font-medium">Nhân viên</span>
        <select
          name="staff_id"
          value={form.staff_id}
          onChange={handleChange}
          className="mt-1 w-full border rounded p-2"
          required
        >
          <option value="">-- Chọn nhân viên --</option>
          {staffOptions.map((s) => (
            <option key={s.id} value={s.id}>
              {s.name}
            </option>
          ))}
        </select>
      </label>

      {/* DAY */}
      <label className="block">
        <span className="font-medium">Ngày trong tuần</span>
        <select
          name="day_of_week"
          value={form.day_of_week}
          onChange={handleChange}
          className="mt-1 w-full border rounded p-2"
        >
          {[
            "Chủ nhật",
            "Thứ 2",
            "Thứ 3",
            "Thứ 4",
            "Thứ 5",
            "Thứ 6",
            "Thứ 7",
          ].map((d, i) => (
            <option value={i} key={i}>
              {d}
            </option>
          ))}
        </select>
      </label>

      {/* TIME RANGE */}
      <div className="grid grid-cols-2 gap-4">
        <label className="block">
          <span className="font-medium">Bắt đầu</span>
          <input
            type="time"
            name="start_time"
            value={form.start_time}
            onChange={handleChange}
            className="mt-1 w-full border rounded p-2"
            required
          />
        </label>

        <label className="block">
          <span className="font-medium">Kết thúc</span>
          <input
            type="time"
            name="end_time"
            value={form.end_time}
            onChange={handleChange}
            className="mt-1 w-full border rounded p-2"
            required
          />
        </label>
      </div>

      {/* IS WORKING */}
      <label className="flex items-center gap-2">
        <input
          type="checkbox"
          name="is_working"
          checked={form.is_working}
          onChange={handleChange}
        />
        <span>Đang làm việc</span>
      </label>

      {/* BUTTON */}
      <button
        type="submit"
        disabled={submitting}
        className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
      >
        {submitting ? "Đang lưu..." : "Lưu lịch"}
      </button>
    </form>
  );
}
