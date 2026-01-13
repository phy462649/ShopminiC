import StaffTable from "../components/StaffTable";

export default function StaffPage() {
  return (
    <div className="h-full overflow-auto" style={{ maxHeight: 'calc(100vh - 280px)' }}>
      <StaffTable />
    </div>
  );
}
