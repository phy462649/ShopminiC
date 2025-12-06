// import { useQuery } from "@tanstack/react-query";
// import { useState } from "react";
// const customers = [
//   {
//     id: 1,
//     name: "Nguyễn Văn A",
//     email: "vana@example.com",
//     phone: "0987000001",
//     address: "Hà Nội",
//     created_at: "2024-01-10T10:00:00",
//   },
//   {
//     id: 2,
//     name: "Trần Thị B",
//     email: "thib@example.com",
//     phone: "0987000002",
//     address: "TP.HCM",
//     created_at: "2024-01-11T11:00:00",
//   },
//   {
//     id: 3,
//     name: "Phạm Văn C",
//     email: "vanc@example.com",
//     phone: "0987000003",
//     address: "Đà Nẵng",
//     created_at: "2024-01-12T12:00:00",
//   },
// ];
// const fetchCustomers = async () => customers; // fake API

// // async function fetchCustomers() {
// // const res = await fetch("/api/customers");
// // if (!res.ok) throw new Error("Failed to load customers");
// // return res.json();
// // }

// export default function Users() {
//   const [formadd, setformadd] = useState(false);
//   const { data, isLoading, isError } = useQuery({
//     queryKey: ["customers"],
//     queryFn: fetchCustomers,
//   });
//   const [selectedId, setSelectedId] = useState(null);
//   const handleAdd = () => setformadd(true);
//   const handleCloseForm = () => setformadd(false);
//   const handleSave = (newCustomer) => {
//     customers((prev) => [
//       ...prev,
//       { ...newCustomer, id: prev.length + 1 }, // tự cấp id
//     ]);
//   };
//   // const { data, isLoading, isError } = useQuery({
//   //   queryKey: ["customers"],
//   //   queryFn: fetchCustomers,
//   // });

//   // // useEffect chỉ để side-effect, ví dụ filter hoặc log
//   // const [filtered, setFiltered] = useState([]);
//   // useEffect(() => {
//   //   if (data) {
//   //     setFiltered(data.filter((c) => c.name.includes("Nguyễn")));
//   //     console.log("Filtered data updated");
//   //   }
//   // }, [data]);
//   if (isLoading) return <p className="p-4">Đang tải...</p>;
//   if (isError) return <p className="p-4 text-red-600">Lỗi tải dữ liệu</p>;
//   // const [search, setSearch] = useState("");

//   return (
//     <div className="p-4">
//       <div className="mb-4 flex items-center">
//         {/* Input search */}
//         <div className="relative w-64 mr-auto">
//           <input
//             type="text"
//             placeholder="Tìm kiếm khách hàng..."
//             className="w-full pl-10 pr-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
//           />
//           <svg
//             className="w-5 h-5 absolute left-3 top-2.5 text-gray-400 pointer-events-none"
//             fill="none"
//             stroke="currentColor"
//             viewBox="0 0 24 24"
//           >
//             <path
//               strokeLinecap="round"
//               strokeLinejoin="round"
//               strokeWidth="2"
//               d="M21 21l-4.35-4.35m0 0A7.5 7.5 0 1010.5 3a7.5 7.5 0 006.15 13.65z"
//             />
//           </svg>
//         </div>

//         {/* Gộp 3 button trong 1 div */}
//         <div className="flex mx-8">
//           <button
//             className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green"
//             onClick={handleAdd}
//           >
//             Thêm
//           </button>
//           <button className="px-4 py-2 bg-yellow-500 text-white rounded-md hover:bg-yellow-600 mx-10">
//             Sửa
//           </button>
//           <button
//             className="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
//             onClick={handelDelete}
//           >
//             Xóa
//           </button>
//         </div>
//       </div>

//       <div className="overflow-x-auto">
//         <table className="min-w-full border border-gray-300 text-base">
//           <thead className="bg-gray-100">
//             <tr>
//               <th className="p-2 border">ID</th>
//               <th className="p-2 border">Tên</th>
//               <th className="p-2 border">Email</th>
//               <th className="p-2 border">SĐT</th>
//               <th className="p-2 border">Địa chỉ</th>
//               <th className="p-2 border">Ngày tạo</th>
//             </tr>
//           </thead>

//           <tbody>
//             {data?.map((c) => (
//               <tr
//                 key={c.id}
//                 className={`hover:bg-gray-50 cursor-pointer ${
//                   selectedId === c.id ? "bg-pink-100" : ""
//                 }`}
//                 onClick={() => setSelectedId(c.id)}
//               >
//                 <td className="p-2 border text-center">{c.id}</td>
//                 <td className="p-2 border text-center">{c.name}</td>
//                 <td className="p-2 border text-center">{c.email || "—"}</td>
//                 <td className="p-2 border text-center">{c.phone || "—"}</td>
//                 <td className="p-2 border text-center">{c.address || "—"}</td>
//                 <td className="p-2 border text-center">
//                   {new Date(c.created_at).toLocaleDateString("vi-VN")}
//                 </td>
//               </tr>
//             ))}
//           </tbody>
//         </table>

//         {formadd && (
//           <CustomerForm onClose={handleCloseForm} onSave={handleSave} />
//         )}
//       </div>
//     </div>
//   );
// }
import { useQuery } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import CustomerForm from "./CustomerForm";
import { message } from "antd";

const initialCustomers = [
  {
    id: 1,
    name: "Nguyễn Văn A",
    email: "vana@example.com",
    phone: "0987000001",
    address: "Hà Nội",
    created_at: "2024-01-10T10:00:00",
  },
  {
    id: 2,
    name: "Trần Thị B",
    email: "thib@example.com",
    phone: "0987000002",
    address: "TP.HCM",
    created_at: "2024-01-11T11:00:00",
  },
  {
    id: 3,
    name: "Phạm Văn C",
    email: "vanc@example.com",
    phone: "0987000003",
    address: "Đà Nẵng",
    created_at: "2024-01-12T12:00:00",
  },
];

const fetchCustomers = async () => initialCustomers;

export default function Users() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ["customers"],
    queryFn: fetchCustomers,
  });

  const [customersList, setCustomersList] = useState([]);
  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingCustomer, setEditingCustomer] = useState(null);
  const emptyCustomer = {
    name: "",
    email: "",
    phone: "",
    address: "",
  };

  useEffect(() => {
    if (data) setCustomersList(data);
  }, [data]);

  const handleAdd = () => {
    setEditingCustomer(emptyCustomer);
    setFormOpen(true);
  };

  const handleUpdate = () => {
    if (!selectedId)
      return message.warning("Please Select a customer to update");

    const customer = customersList.find((c) => c.id === selectedId);
    if (!customer) return;

    setEditingCustomer(customer);
    setFormOpen(true);
  };

  const handleDelete = () => {
    if (!selectedId) {
      message.error("Please Select a customer to delete");
      return;
    }

    setCustomersList((prev) => prev.filter((c) => c.id !== selectedId));
    setSelectedId(null);

    message.success("Đã xóa khách hàng!");
  };

  const handleCloseForm = () => setFormOpen(false);

  const handleSave = (customer) => {
    if (editingCustomer) {
      // update
      setCustomersList((prev) =>
        prev.map((c) =>
          c.id === editingCustomer.id ? { ...c, ...customer } : c
        )
      );
    } else {
      // add mới
      setCustomersList((prev) => [
        ...prev,
        {
          ...customer,
          id: prev.length ? Math.max(...prev.map((c) => c.id)) + 1 : 1,
          created_at: new Date().toISOString(),
        },
      ]);
    }

    setFormOpen(false);
    setEditingCustomer(null);
  };

  if (isLoading) return <p className="p-4">Đang tải...</p>;
  if (isError) return <p className="p-4 text-red-600">Lỗi tải dữ liệu</p>;

  return (
    <div className="p-4">
      {/* Toolbar */}
      <div className="mb-4 flex items-center">
        <div className="relative w-64 mr-auto">
          <input
            type="text"
            placeholder="Tìm kiếm khách hàng..."
            className="w-full pl-10 pr-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
          />
          <svg
            className="w-5 h-5 absolute left-3 top-2.5 text-gray-400"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              d="M21 21l-4.35-4.35m0 0A7.5 7.5 0 1010.5 3a7.5 7.5 0 006.15 13.65z"
            />
          </svg>
        </div>

        <div className="flex mx-8">
          <button
            className="px-4 py-2 bg-green-500 text-white rounded-md"
            onClick={handleAdd}
          >
            Thêm
          </button>

          <button
            className="px-4 py-2 bg-yellow-500 text-white rounded-md mx-4"
            onClick={handleUpdate}
          >
            Sửa
          </button>

          <button
            className="px-4 py-2 bg-red-500 text-white rounded-md"
            onClick={handleDelete}
          >
            Xóa
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="overflow-x-auto">
        <table className="min-w-full border border-gray-300 text-base">
          <thead className="bg-gray-100">
            <tr>
              <th className="p-2 border">ID</th>
              <th className="p-2 border">Tên</th>
              <th className="p-2 border">Email</th>
              <th className="p-2 border">SĐT</th>
              <th className="p-2 border">Địa chỉ</th>
              <th className="p-2 border">Ngày tạo</th>
            </tr>
          </thead>

          <tbody>
            {customersList.map((c) => (
              <tr
                key={c.id}
                className={`cursor-pointer hover:bg-gray-50 ${
                  selectedId === c.id ? "bg-pink-100" : ""
                }`}
                onClick={() => setSelectedId(c.id)}
              >
                <td className="p-2 border text-center">{c.id}</td>
                <td className="p-2 border text-center">{c.name}</td>
                <td className="p-2 border text-center">{c.email}</td>
                <td className="p-2 border text-center">{c.phone}</td>
                <td className="p-2 border text-center">{c.address}</td>
                <td className="p-2 border text-center">
                  {new Date(c.created_at).toLocaleDateString("vi-VN")}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Form Add / Edit */}
      {formOpen && (
        <CustomerForm
          initialData={editingCustomer} // null = thêm mới
          onClose={handleCloseForm}
          onSave={handleSave}
        />
      )}
    </div>
  );
}
