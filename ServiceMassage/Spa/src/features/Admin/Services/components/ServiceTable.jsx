import { useState } from "react";
import { message, Modal } from "antd";
import ServiceForm from "./ServiceForm";
import {
  useService,
  useCreateService,
  useUpdateService,
  useDeleteService,
} from "../hooks/useService";

export default function ServiceTable() {
  const { data: services = [], isLoading, isError } = useService();
  const createMutation = useCreateService();
  const updateMutation = useUpdateService();
  const deleteMutation = useDeleteService();

  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingService, setEditingService] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  const filteredServices = services.filter((s) =>
    [s.name, s.description]
      .filter(Boolean)
      .some((field) => field.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const handleAdd = () => {
    setEditingService(null);
    setFormOpen(true);
  };

  const handleEdit = () => {
    if (!selectedId) return message.warning("Please select a service to edit");
    const service = services.find((s) => s.id === selectedId);
    if (service) {
      setEditingService(service);
      setFormOpen(true);
    }
  };

  const handleDelete = () => {
    if (!selectedId) return message.warning("Please select a service to delete");
    Modal.confirm({
      title: "Confirm Delete",
      content: "Are you sure you want to delete this service?",
      okText: "Delete",
      cancelText: "Cancel",
      okButtonProps: { danger: true },
      onOk: () => {
        deleteMutation.mutate(selectedId, {
          onSuccess: () => setSelectedId(null),
        });
      },
    });
  };

  const handleSave = (data) => {
    if (editingService) {
      updateMutation.mutate(
        { id: editingService.id, data },
        { onSuccess: () => setFormOpen(false) }
      );
    } else {
      createMutation.mutate(data, {
        onSuccess: () => setFormOpen(false),
      });
    }
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
          <button onClick={handleAdd} className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600">
            Add
          </button>
          <button onClick={handleEdit} className="px-4 py-2 bg-yellow-500 text-white rounded-md hover:bg-yellow-600">
            Edit
          </button>
          <button onClick={handleDelete} className="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600">
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
              <th className="p-3 text-center font-semibold">Image</th>
              <th className="p-3 text-center font-semibold">Name</th>
              <th className="p-3 text-center font-semibold">Description</th>
              <th className="p-3 text-center font-semibold">Duration</th>
              <th className="p-3 text-center font-semibold">Price</th>
              {/* <th className="p-3 text-center font-semibold">Created At</th> */}
              {/* <th className="p-3 text-center font-semibold">Updated At</th> */}
            </tr>
          </thead>
          <tbody>
            {filteredServices.length > 0 ? (
              filteredServices.map((s) => (
                <tr
                  key={s.id}
                  onClick={() => setSelectedId(s.id)}
                  className={`cursor-pointer border-b hover:bg-gray-50 ${selectedId === s.id ? "bg-pink-100" : ""}`}
                >
                  <td className="p-3 text-center font-medium">{s.id}</td>
                  <td className="p-3 text-center">
                    {s.urlImage ? (
                      <img src={s.urlImage} alt={s.name} className="w-12 h-12 object-cover rounded mx-auto" />
                    ) : (
                      <div className="w-12 h-12 bg-gray-200 rounded mx-auto flex items-center justify-center text-gray-400 text-xs">
                        No img
                      </div>
                    )}
                  </td>
                  <td className="p-3 text-center">{s.name}</td>
                  <td className="p-3 text-center">{s.description || "-"}</td>
                  <td className="p-3 text-center">{s.duration_minutes || s.durationMinutes} min</td>
                  <td className="p-3 text-center font-semibold text-pink-600">
                    ${s.price?.toLocaleString("en-US")}
                  </td>
                  {/* <td className="p-3 text-center">
                    {s.created_at || s.createdAt ? new Date(s.created_at || s.createdAt).toLocaleDateString("en-US") : "-"}
                  </td>
                  <td className="p-3 text-center">
                    {s.updated_at || s.updatedAt ? new Date(s.updated_at || s.updatedAt).toLocaleDateString("en-US") : "-"}
                  </td> */}
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="8" className="p-4 text-center text-gray-500">
                  No data available
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Form Modal */}
      {formOpen && (
        <ServiceForm
          key={editingService?.id || 'new'}
          initialData={editingService}
          onClose={() => setFormOpen(false)}
          onSave={handleSave}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      )}
    </div>
  );
}
