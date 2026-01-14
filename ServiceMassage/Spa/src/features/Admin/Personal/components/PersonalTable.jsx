import { useState } from "react";
import { message, Tag, Modal } from "antd";
import PersonalForm from "./PersonalForm";
import {
  usePersonal,
  useCreatePersonal,
  useUpdatePersonal,
  useDeletePersonal,
} from "../hooks/usePersonal";

export default function PersonalTable() {
  const { data: personals = [], isLoading, isError } = usePersonal();
  const createMutation = useCreatePersonal();
  const updateMutation = useUpdatePersonal();
  const deleteMutation = useDeletePersonal();

  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingPersonal, setEditingPersonal] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  const filteredPersonals = personals.filter((p) =>
    [p.name, p.username, p.email, p.phone, p.address]
      .filter(Boolean)
      .some((field) => field.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const handleAdd = () => {
    setEditingPersonal(null);
    setFormOpen(true);
  };

  const handleEdit = () => {
    if (!selectedId) return message.warning("Please select a user to edit");
    const personal = personals.find((p) => p.id === selectedId);
    if (personal) {
      setEditingPersonal(personal);
      setFormOpen(true);
    }
  };

  const handleDelete = () => {
    if (!selectedId) return message.warning("Please select a user to delete");
    Modal.confirm({
      title: "Confirm Delete",
      content: "Are you sure you want to delete this user?",
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
    if (editingPersonal) {
      updateMutation.mutate(
        { id: editingPersonal.id, data },
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
              <th className="p-3 text-center font-semibold">Name</th>
              <th className="p-3 text-center font-semibold">Username</th>
              <th className="p-3 text-center font-semibold">Email</th>
              <th className="p-3 text-center font-semibold">Phone</th>
              <th className="p-3 text-center font-semibold">Address</th>
              <th className="p-3 text-center font-semibold">Role</th>
              <th className="p-3 text-center font-semibold">Verified</th>
              {/* <th className="p-3 text-center font-semibold">Created At</th> */}
            </tr>
          </thead>
          <tbody>
            {filteredPersonals.length > 0 ? (
              filteredPersonals.map((p) => (
                <tr
                  key={p.id}
                  onClick={() => setSelectedId(p.id)}
                  className={`cursor-pointer border-b ${selectedId === p.id ? "bg-pink-100" : "hover:bg-gray-50"}`}
                >
                  <td className="p-3 text-center font-medium">{p.id}</td>
                  <td className="p-3 text-center">{p.name}</td>
                  <td className="p-3 text-center">{p.username}</td>
                  <td className="p-3 text-center">{p.email}</td>
                  <td className="p-3 text-center">{p.phone}</td>
                  <td className="p-3 text-center">{p.address}</td>
                  <td className="p-3 text-center">
                    <Tag color={p.roleName === "ADMIN" ? "red" : "blue"}>
                      {p.roleName}
                    </Tag>
                  </td>
                  <td className="p-3 text-center">
                    <Tag color={p.statusVerify ? "green" : "orange"}>
                      {p.statusVerify ? "Verified" : "Pending"}
                    </Tag>
                  </td>
                  {/* <td className="p-3 text-center">
                    {p.createdAt ? new Date(p.createdAt).toLocaleDateString("en-US") : "-"}
                  </td> */}
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="9" className="p-4 text-center text-gray-500">
                  No data available
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Form Modal */}
      {formOpen && (
        <PersonalForm
          key={editingPersonal?.id || 'new'}
          initialData={editingPersonal}
          onClose={() => setFormOpen(false)}
          onSave={handleSave}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      )}
    </div>
  );
}
