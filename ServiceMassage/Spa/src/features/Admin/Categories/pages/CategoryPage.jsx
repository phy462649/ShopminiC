import CategoryTable from "../components/CategoryTable";

export default function CategoryPage() {
  return (
    <div className="h-full overflow-auto" style={{ maxHeight: 'calc(100vh - 280px)' }}>
      <CategoryTable />
    </div>
  );
}
