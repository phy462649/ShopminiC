import { useState, useMemo } from 'react';
import { useBooking } from '../hooks/useBooking';
import { Tag } from 'antd';

const statusColors = {
  0: 'bg-orange-100 border-orange-300 text-orange-700',
  1: 'bg-blue-100 border-blue-300 text-blue-700',
  2: 'bg-green-100 border-green-300 text-green-700',
  3: 'bg-red-100 border-red-300 text-red-700',
  pending: 'bg-orange-100 border-orange-300 text-orange-700',
  confirmed: 'bg-blue-100 border-blue-300 text-blue-700',
  completed: 'bg-green-100 border-green-300 text-green-700',
  cancelled: 'bg-red-100 border-red-300 text-red-700',
};

export default function BookingCalendar() {
  const { data: bookings = [] } = useBooking();
  const [currentDate, setCurrentDate] = useState(new Date());

  const year = currentDate.getFullYear();
  const month = currentDate.getMonth();

  const daysInMonth = new Date(year, month + 1, 0).getDate();
  const firstDayOfMonth = new Date(year, month, 1).getDay();

  const bookingsByDate = useMemo(() => {
    const grouped = {};
    bookings.forEach((booking) => {
      if (booking.startTime) {
        const date = new Date(booking.startTime).toDateString();
        if (!grouped[date]) grouped[date] = [];
        grouped[date].push(booking);
      }
    });
    return grouped;
  }, [bookings]);

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

    for (let i = 0; i < firstDayOfMonth; i++) {
      days.push(<div key={`empty-${i}`} className="h-24 bg-gray-50" />);
    }

    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(year, month, day);
      const dateStr = date.toDateString();
      const dayBookings = bookingsByDate[dateStr] || [];
      const isToday = dateStr === today;

      days.push(
        <div
          key={day}
          className={`h-24 border p-1 overflow-hidden ${isToday ? 'bg-pink-50 border-pink-300' : 'bg-white'}`}
        >
          <div className={`text-sm font-medium mb-1 ${isToday ? 'text-pink-600' : 'text-gray-700'}`}>
            {day}
          </div>
          <div className="space-y-1 overflow-y-auto max-h-16">
            {dayBookings.slice(0, 3).map((booking) => (
              <div
                key={booking.id}
                className={`text-xs px-1 py-0.5 rounded border truncate ${statusColors[booking.status] || statusColors.pending}`}
                title={`${booking.customerName || 'Customer'} - ${booking.serviceName || 'Service'}`}
              >
                {new Date(booking.startTime).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })}
                {' '}{booking.customerName || 'Customer'}
              </div>
            ))}
            {dayBookings.length > 3 && (
              <div className="text-xs text-gray-500 text-center">
                +{dayBookings.length - 3} more
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
        <Tag color="orange">Pending</Tag>
        <Tag color="blue">Confirmed</Tag>
        <Tag color="green">Completed</Tag>
        <Tag color="red">Cancelled</Tag>
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
