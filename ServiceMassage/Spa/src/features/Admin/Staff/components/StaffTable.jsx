import { useState } from "react";
import { message } from "antd";
import { useStaff } from "../hooks/useStaff";

export default function StaffTable() {
  const { data: staffList = [], isLoading, isError } = useStaff();

  const [selectedId, setSelectedId] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  const filteredStaff = staffList.filter((s) =>
    [s.name, s.email, s.phone]
      .filter(Boolean)
      .some((field) => field.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const handleAdd = () => {
    message.info("Please add staff in User Management page and assign Staff role");
  };

  const handleEdit = () => {
    if (!selectedId) return message.warning("Please select a staff to edit");
    message.info("Please edit staff info in User Management page");
  };

  const handleDelete = () => {
    if (!selectedId) return message.warning("Please select a staff to delete");
    message.info("Please delete staff in User Management page");
  };

  if (isLoading) return <div className="p-4 text-center">Loading...</div>;
  if (isError) return <div className="p-4 text-center text-red-500">Error loading data</div>;

  return (
    <div className="p-4">
      {/* Toolbar */}
      <div className="mb-4 flex flex-wrap items-center justify-between gap-4">
        <div className="relative min-w-[200px] max-w-xs">
          <input
            type="text"
            placeholder="Search..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
          />
          <svg className="w-5 h-5 absolute left-3 top-2.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-4.35-4.35m0 0A7.5 7.5 0 1010.5 3a7.5 7.5 0 006.15 13.65z" />
          </svg>
        </div>

        <div className="flex gap-4">
          <button 
            onClick={handleAdd} 
            className="px-4 py-2 bg-green-500 text-white rounded-md opacity-50 cursor-not-allowed"
            title="Manage in User page"
          >
            Add
          </button>
          <button 
            onClick={handleEdit} 
            className="px-4 py-2 bg-yellow-500 text-white rounded-md opacity-50 cursor-not-allowed"
            title="Manage in User page"
          >
            Edit
          </button>
          <button 
            onClick={handleDelete} 
            className="px-4 py-2 bg-red-500 text-white rounded-md opacity-50 cursor-not-allowed"
            title="Manage in User page"
          >
            Delete
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="overflow-x-auto bg-white rounded-lg shadow">
        <table className="min-w-full text-sm">
          <thead className="bg-pink-50">
            <tr>
              <th className="p-3 text-center font-semibold">ID</th>
              <th className="p-3 text-center font-semibold">Name</th>
              <th className="p-3 text-center font-semibold">Phone</th>
              <th className="p-3 text-center font-semibold">Email</th>
              <th className="p-3 text-center font-semibold">Created At</th>
              <th className="p-3 text-center font-semibold">Updated At</th>
            </tr>
          </thead>
          <tbody>
            {filteredStaff.length > 0 ? (
              filteredStaff.map((s) => (
                <tr
                  key={s.id}
                  onClick={() => setSelectedId(s.id)}
                  className={`cursor-pointer border-b hover:bg-gray-50 ${selectedId === s.id ? "bg-pink-100" : ""}`}
                >
                  <td className="p-3 text-center font-medium">{s.id}</td>
                  <td className="p-3 text-center">{s.name}</td>
                  <td className="p-3 text-center">{s.phone || "-"}</td>
                  <td className="p-3 text-center">{s.email || "-"}</td>
                  <td className="p-3 text-center">
                    {s.createdAt ? new Date(s.createdAt).toLocaleString("en-US") : "-"}
                  </td>
                  <td className="p-3 text-center">
                    {s.updatedAt ? new Date(s.updatedAt).toLocaleString("en-US") : "-"}
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="6" className="p-4 text-center text-gray-500">
                  No data available
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
