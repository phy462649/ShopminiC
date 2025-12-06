import { useQuery } from "@tanstack/react-query";
// payments.mock.js
const payments = [
  {
    id: 1,
    payment_type: "booking",
    reference_id: 101,
    amount: 500000,
    method: "momo",
    status: "completed",
    payment_time: "2024-02-10T10:30:00",
    updated_at: "2024-02-10T10:30:00",
    booking_id: 101,
    order_id: null,
    created_by: "admin",
  },
  {
    id: 2,
    payment_type: "order",
    reference_id: 502,
    amount: 1250000,
    method: "cash",
    status: "pending",
    payment_time: "2024-02-11T11:00:00",
    updated_at: "2024-02-11T11:00:00",
    booking_id: null,
    order_id: 502,
    created_by: "staff01",
  },
  {
    id: 3,
    payment_type: "booking",
    reference_id: 103,
    amount: 300000,
    method: "bank_transfer",
    status: "failed",
    payment_time: "2024-02-12T09:15:00",
    updated_at: "2024-02-12T09:15:00",
    booking_id: 103,
    order_id: null,
    created_by: "admin",
  },
  {
    id: 4,
    payment_type: "order",
    reference_id: 510,
    amount: 950000,
    method: "card",
    status: "completed",
    payment_time: "2024-02-13T14:20:00",
    updated_at: "2024-02-13T14:20:00",
    booking_id: null,
    order_id: 510,
    created_by: "staff02",
  },
];

export default function PaymentTable() {
  const fetchOrders = async () => payments;
  const { data, isLoading, isError } = useQuery({
    queryKey: ["payments"],
    queryFn: fetchOrders,
  });

  if (isLoading) return <p className="p-4">Đang tải...</p>;
  if (isError) return <p className="p-4 text-red-600">Lỗi tải dữ liệu</p>;

  return (
    <div className="p-4">
      <h2 className="text-xl font-semibold mb-4">Danh sách thanh toán</h2>

      <div className="overflow-x-auto">
        <table className="min-w-full border text-sm">
          <thead className="bg-gray-100">
            <tr>
              <th className="p-2 border">ID</th>
              <th className="p-2 border">Loại</th>
              <th className="p-2 border">Reference</th>
              <th className="p-2 border">Amount</th>
              <th className="p-2 border">Method</th>
              <th className="p-2 border">Status</th>
              <th className="p-2 border">Thời gian</th>
              <th className="p-2 border">Action</th>
            </tr>
          </thead>

          <tbody>
            {data?.map((p) => (
              <tr key={p.id} className="hover:bg-gray-50">
                <td className="p-2 border">{p.id}</td>
                <td className="p-2 border capitalize">{p.payment_type}</td>
                <td className="p-2 border">{p.booking_id || p.order_id}</td>
                <td className="p-2 border">{Number(p.amount).toFixed(2)}</td>
                <td className="p-2 border capitalize">{p.method}</td>

                <td className="p-2 border">
                  <span
                    className={
                      "px-2 py-1 rounded text-white text-xs " +
                      (p.status === "completed"
                        ? "bg-green-600"
                        : p.status === "failed"
                        ? "bg-red-600"
                        : "bg-yellow-600")
                    }
                  >
                    {p.status}
                  </span>
                </td>

                <td className="p-2 border">
                  {new Date(p.payment_time).toLocaleString("vi-VN")}
                </td>

                <td className="p-2 border">
                  <button
                    className="px-3 py-1 text-sm bg-blue-600 text-white rounded hover:bg-blue-700"
                    aria-label={`Xem chi tiết thanh toán ${p.id}`}
                  >
                    Detail
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
