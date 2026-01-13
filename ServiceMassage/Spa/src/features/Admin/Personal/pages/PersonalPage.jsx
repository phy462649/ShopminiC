import PersonalTable from "../components/PersonalTable";

export default function PersonalPage() {
  return (
    <div className="h-full overflow-auto" style={{ maxHeight: 'calc(100vh - 280px)' }}>
      <PersonalTable />
    </div>
  );
}
