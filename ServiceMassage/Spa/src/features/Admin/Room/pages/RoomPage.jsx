import RoomTable from "../components/RoomTable";

export default function RoomPage() {
  return (
    <div className="h-full overflow-auto" style={{ maxHeight: 'calc(100vh - 280px)' }}>
      <RoomTable />
    </div>
  );
}
