import { useState } from 'react';
import { Tabs } from 'antd';
import BookingTable from '../components/BookingTable';
import BookingCalendar from '../components/BookingCalendar';
import BookingStatistics from '../components/BookingStatistics';

export default function BookingPage() {
  const [activeTab, setActiveTab] = useState('list');

  const items = [
    {
      key: 'list',
      label: 'List View',
      children: (
        <div className="h-[calc(100vh-100px)] overflow-y-auto">
          <BookingTable />
        </div>
      ),
    },
    {
      key: 'calendar',
      label: 'Calendar',
      children: (
        <div className="h-[calc(100vh-100px)] overflow-y-auto">
          <BookingCalendar />
        </div>
      ),
    },
    {
      key: 'statistics',
      label: 'Statistics',
      children: (
        <div className="h-[calc(50vh-100px)] overflow-y-auto">
          <BookingStatistics />
        </div>
      ),
    },
  ];

  return (
    <div className="h-full flex flex-col">
      <Tabs
        activeKey={activeTab}
        onChange={setActiveTab}
        items={items}
        className="bg-white rounded-lg p-4 flex-1"
      />
    </div>
  );
}
