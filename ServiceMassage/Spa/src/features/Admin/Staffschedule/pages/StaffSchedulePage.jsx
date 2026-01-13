import StaffScheduleTable from "../components/StaffscheduleTable";

export default function StaffSchedulePage() {
  return (
    <div className="h-full overflow-auto" style={{ maxHeight: 'calc(100vh - 280px)' }}>
      <StaffScheduleTable />
    </div>
  );
}
