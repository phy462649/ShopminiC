import OrderTable from "../components/OrderTable";

export default function OrderPage() {
  return (
    <div className="h-full overflow-auto" style={{ maxHeight: 'calc(100vh - 280px)' }}>
      <OrderTable />
    </div>
  );
}
