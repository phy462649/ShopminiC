import { useMemo } from 'react';
import { useBooking } from '../hooks/useBooking';

export default function BookingStatistics() {
  const { data: bookings = [], isLoading } = useBooking();

  const stats = useMemo(() => {
    const total = bookings.length;
    const pending = bookings.filter((b) => b.status === 0 || b.status === 'pending').length;
    const confirmed = bookings.filter((b) => b.status === 1 || b.status === 'confirmed').length;
    const completed = bookings.filter((b) => b.status === 2 || b.status === 'completed').length;
    const cancelled = bookings.filter((b) => b.status === 3 || b.status === 'cancelled').length;

    const today = new Date().toDateString();
    const todayBookings = bookings.filter(
      (b) => b.startTime && new Date(b.startTime).toDateString() === today
    ).length;

    const now = new Date();
    const startOfWeek = new Date(now.setDate(now.getDate() - now.getDay()));
    const weekBookings = bookings.filter((b) => {
      if (!b.startTime) return false;
      return new Date(b.startTime) >= startOfWeek;
    }).length;

    const totalRevenue = bookings
      .filter((b) => b.status === 2 || b.status === 'completed')
      .reduce((sum, b) => sum + (b.totalAmount || 0), 0);

    return {
      total,
      pending,
      confirmed,
      completed,
      cancelled,
      todayBookings,
      weekBookings,
      totalRevenue,
      completionRate: total > 0 ? ((completed / total) * 100).toFixed(1) : 0,
      cancellationRate: total > 0 ? ((cancelled / total) * 100).toFixed(1) : 0,
    };
  }, [bookings]);

  if (isLoading) return <div className="p-4 text-center">Loading...</div>;

  const statCards = [
    { label: 'Total Bookings', value: stats.total, color: 'bg-blue-500', icon: '📋' },
    { label: 'Today', value: stats.todayBookings, color: 'bg-pink-500', icon: '📅' },
    { label: 'This Week', value: stats.weekBookings, color: 'bg-purple-500', icon: '📆' },
    { label: 'Pending', value: stats.pending, color: 'bg-orange-500', icon: '⏳' },
    { label: 'Confirmed', value: stats.confirmed, color: 'bg-blue-400', icon: '✓' },
    { label: 'Completed', value: stats.completed, color: 'bg-green-500', icon: '✅' },
    { label: 'Cancelled', value: stats.cancelled, color: 'bg-red-500', icon: '❌' },
    { label: 'Total Revenue', value: `$${stats.totalRevenue.toLocaleString('en-US')}`, color: 'bg-teal-500', icon: '💰' },
  ];

  return (
    <div className="space-y-6">
      {/* Stats Grid */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {statCards.map((stat, index) => (
          <div key={index} className="bg-white rounded-lg shadow p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-500 text-sm">{stat.label}</p>
                <p className="text-2xl font-bold text-gray-800">{stat.value}</p>
              </div>
              <div className={`w-12 h-12 ${stat.color} rounded-full flex items-center justify-center text-white text-xl`}>
                {stat.icon}
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Status Distribution */}
      <div className="bg-white rounded-lg shadow p-4">
        <h3 className="text-lg font-semibold mb-4">Status Distribution</h3>
        <div className="space-y-3">
          {[
            { label: 'Pending', value: stats.pending, total: stats.total, color: 'bg-orange-500' },
            { label: 'Confirmed', value: stats.confirmed, total: stats.total, color: 'bg-blue-500' },
            { label: 'Completed', value: stats.completed, total: stats.total, color: 'bg-green-500' },
            { label: 'Cancelled', value: stats.cancelled, total: stats.total, color: 'bg-red-500' },
          ].map((item, index) => {
            const percentage = item.total > 0 ? (item.value / item.total) * 100 : 0;
            return (
              <div key={index}>
                <div className="flex justify-between text-sm mb-1">
                  <span>{item.label}</span>
                  <span>{item.value} ({percentage.toFixed(1)}%)</span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2">
                  <div className={`${item.color} h-2 rounded-full transition-all`} style={{ width: `${percentage}%` }} />
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {/* Recent Bookings */}
      <div className="bg-white rounded-lg shadow p-4">
        <h3 className="text-lg font-semibold mb-4">Recent Bookings</h3>
        <div className="space-y-2">
          {bookings.slice(0, 5).map((booking) => (
            <div key={booking.id} className="flex items-center justify-between p-2 bg-gray-50 rounded">
              <div>
                <p className="font-medium">{booking.customerName || `Customer #${booking.customerId}`}</p>
                <p className="text-sm text-gray-500">
                  {booking.startTime ? new Date(booking.startTime).toLocaleString('en-US') : '-'}
                </p>
              </div>
              <div className="text-right">
                <span className={`px-2 py-1 rounded text-xs ${
                  (booking.status === 2 || booking.status === 'completed') ? 'bg-green-100 text-green-700' :
                  (booking.status === 1 || booking.status === 'confirmed') ? 'bg-blue-100 text-blue-700' :
                  (booking.status === 3 || booking.status === 'cancelled') ? 'bg-red-100 text-red-700' :
                  'bg-orange-100 text-orange-700'
                }`}>
                  {booking.status === 0 || booking.status === 'pending' ? 'Pending' :
                   booking.status === 1 || booking.status === 'confirmed' ? 'Confirmed' :
                   booking.status === 2 || booking.status === 'completed' ? 'Completed' : 'Cancelled'}
                </span>
                <p className="text-sm font-semibold text-pink-600 mt-1">
                  ${booking.totalAmount?.toLocaleString('en-US')}
                </p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
