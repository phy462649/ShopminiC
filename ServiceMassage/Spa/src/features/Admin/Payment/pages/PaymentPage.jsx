import PaymentTable from "../components/PaymentTable";

export default function PaymentPage() {
  return (
    <div className="h-full overflow-auto" style={{ maxHeight: 'calc(100vh - 280px)' }}>
      <PaymentTable />
    </div>
  );
}
