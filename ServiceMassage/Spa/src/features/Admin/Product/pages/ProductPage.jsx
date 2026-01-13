import ProductTable from "../components/ProductTable";

export default function ProductPage() {
  return (
    <div className="h-full overflow-auto" style={{ maxHeight: 'calc(100vh - 280px)' }}>
      <ProductTable />
    </div>
  );
}
