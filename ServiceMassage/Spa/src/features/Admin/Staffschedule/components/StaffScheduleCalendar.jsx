import { useState, useMemo } from 'react';
import { useStaffSchedule } from '../hooks/useStaffschedule';
import { Tag } from 'antd';

const dayColors = {
  working: 'bg-green-100 border-green-300 text-green-700',
  off: 'bg-gray-100 border-gray-300 text-gray-500',
};

export default function StaffScheduleCalendar() {
  const { data: schedules = [] } = useStaffSchedule();
  const [currentDate, setCurrentDate] = useState(new Date());

  const year = currentDate.getFullYear();
  const month = currentDate.getMonth();

  const daysInMonth = new Date(year, month + 1, 0).getDate();
  const firstDayOfMonth = new Date(year, month, 1).getDay();

  // Group schedules by day of week (0=Sunday, 1=Monday, etc.)
  const schedulesByDayOfWeek = useMemo(() => {
    const grouped = {};
    schedules.forEach((schedule) => {
      const dayOfWeek = schedule.dayOfWeek;
      if (!grouped[dayOfWeek]) grouped[dayOfWeek] = [];
      grouped[dayOfWeek].push(schedule);
    });
    return grouped;
  }, [schedules]);

  const prevMonth = () => setCurrentDate(new Date(year, month - 1, 1));
  const nextMonth = () => setCurrentDate(new Date(year, month + 1, 1));
  const goToToday = () => setCurrentDate(new Date());

  const monthNames = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];

  const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  const renderCalendarDays = () => {
    const days = [];
    const today = new Date().toDateString();

    // Empty cells for days before the first day of month
    for (let i = 0; i < firstDayOfMonth; i++) {
      days.push(<div key={`empty-${i}`} className="h-28 bg-gray-50" />);
    }

    // Days of the month
    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(year, month, day);
      const dateStr = date.toDateString();
      const dayOfWeek = date.getDay(); // 0=Sunday, 1=Monday, etc.
      const daySchedules = schedulesByDayOfWeek[dayOfWeek] || [];
      const isToday = dateStr === today;

      days.push(
        <div
          key={day}
          className={`h-28 border p-1 overflow-hidden ${isToday ? 'bg-pink-50 border-pink-300' : 'bg-white'}`}
        >
          <div className={`text-sm font-medium mb-1 ${isToday ? 'text-pink-600' : 'text-gray-700'}`}>
            {day}
          </div>
          <div className="space-y-1 overflow-y-auto max-h-20">
            {daySchedules.slice(0, 3).map((schedule) => (
              <div
                key={schedule.id}
                className={`text-xs px-1 py-0.5 rounded border truncate ${schedule.isWorking ? dayColors.working : dayColors.off}`}
                title={`${schedule.staffName} - ${schedule.startTime} to ${schedule.endTime}`}
              >
                <span className="font-medium">{schedule.staffName}</span>
                <br />
                {schedule.isWorking ? `${schedule.startTime?.slice(0, 5)} - ${schedule.endTime?.slice(0, 5)}` : 'Off'}
              </div>
            ))}
            {daySchedules.length > 3 && (
              <div className="text-xs text-gray-500 text-center">
                +{daySchedules.length - 3} more
              </div>
            )}
          </div>
        </div>
      );
    }

    return days;
  };

  return (
    <div className="bg-white rounded-lg">
      {/* Header */}
      <div className="flex items-center justify-between p-4 border-b">
        <div className="flex items-center gap-2">
          <button onClick={prevMonth} className="p-2 hover:bg-gray-100 rounded">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <h2 className="text-lg font-semibold min-w-[150px] text-center">
            {monthNames[month]} {year}
          </h2>
          <button onClick={nextMonth} className="p-2 hover:bg-gray-100 rounded">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
          </button>
        </div>
        <button onClick={goToToday} className="px-3 py-1 text-sm bg-pink-500 text-white rounded hover:bg-pink-600">
          Today
        </button>
      </div>

      {/* Legend */}
      <div className="flex gap-4 p-3 border-b text-sm">
        <Tag color="green">Working</Tag>
        <Tag color="default">Off</Tag>
      </div>

      {/* Calendar Grid */}
      <div className="grid grid-cols-7">
        {dayNames.map((day) => (
          <div key={day} className="p-2 text-center font-medium bg-gray-100 border">
            {day}
          </div>
        ))}
        {renderCalendarDays()}
      </div>
    </div>
  );
}
