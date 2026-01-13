import ServiceTable from "../components/ServiceTable";

export default function ServicePage() {
  return (
    <div className="h-full overflow-auto" style={{ maxHeight: 'calc(100vh - 280px)' }}>
      <ServiceTable />
    </div>
  );
}
