import { useState } from 'react';
import { Tabs } from 'antd';
import StaffScheduleTable from '../components/StaffscheduleTable';
import StaffScheduleCalendar from '../components/StaffScheduleCalendar';

const { TabPane } = Tabs;

export default function StaffSchedulePage() {
  const [activeTab, setActiveTab] = useState('table');

  return (
    <div className="h-full overflow-auto p-4" style={{ maxHeight: 'calc(100vh - 140px)' }}>
      <Tabs activeKey={activeTab} onChange={setActiveTab} className="h-full">
        <TabPane tab="Schedule List" key="table">
          <StaffScheduleTable />
        </TabPane>
        <TabPane tab="Calendar View" key="calendar">
          <StaffScheduleCalendar />
        </TabPane>
      </Tabs>
    </div>
  );
}
