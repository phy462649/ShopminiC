import { useState } from "react";
import { message, Tag, Modal, Select } from "antd";
import PersonalForm from "./PersonalForm";
import {
  usePersonal,
  useCreatePersonal,
  useUpdatePersonal,
  useDeletePersonal,
  useSearchPersonal,
} from "../hooks/usePersonal";

export default function PersonalTable() {
  const { data: personals = [], isLoading, isError, refetch } = usePersonal();
  const createMutation = useCreatePersonal();
  const updateMutation = useUpdatePersonal();
  const deleteMutation = useDeletePersonal();
  const searchMutation = useSearchPersonal();

  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingPersonal, setEditingPersonal] = useState(null);
  
  // Search params
  const [searchParams, setSearchParams] = useState({
    name: '',
    email: '',
    phone: '',
    roleId: null,
    sortBy: 'CreatedAt',
    sortOrder: 'desc',
    page: 1,
    pageSize: 10
  });

  const handleSearchChange = (field, value) => {
    setSearchParams(prev => ({ ...prev, [field]: value }));
  };

  const handleSearch = () => {
    const params = {};
    if (searchParams.name) params.name = searchParams.name;
    if (searchParams.email) params.email = searchParams.email;
    if (searchParams.phone) params.phone = searchParams.phone;
    if (searchParams.roleId) params.roleId = searchParams.roleId;
    params.sortBy = searchParams.sortBy;
    params.sortOrder = searchParams.sortOrder;
    params.page = searchParams.page;
    params.pageSize = searchParams.pageSize;
    
    searchMutation.mutate(params);
  };

  const handleReset = () => {
    setSearchParams({
      name: '',
      email: '',
      phone: '',
      roleId: null,
      sortBy: 'CreatedAt',
      sortOrder: 'desc',
      page: 1,
      pageSize: 10
    });
    refetch();
  };

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
      {/* Search Filters */}
      <div className="mb-4 p-4 bg-gray-50 rounded-lg">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-4">
          <input
            type="text"
            placeholder="Search by name..."
            value={searchParams.name}
            onChange={(e) => handleSearchChange('name', e.target.value)}
            className="px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
          />
          <input
            type="text"
            placeholder="Search by email..."
            value={searchParams.email}
            onChange={(e) => handleSearchChange('email', e.target.value)}
            className="px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
          />
          <input
            type="text"
            placeholder="Search by phone..."
            value={searchParams.phone}
            onChange={(e) => handleSearchChange('phone', e.target.value)}
            className="px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-pink-500"
          />
          <Select
            placeholder="Filter by role"
            value={searchParams.roleId}
            onChange={(value) => handleSearchChange('roleId', value)}
            allowClear
            className="w-full"
            options={[
              { value: 1, label: 'ADMIN' },
              { value: 2, label: 'USER' },
              { value: 3, label: 'STAFF' },
            ]}
          />
        </div>
        <div className="flex gap-2">
          <button 
            onClick={handleSearch}
            disabled={searchMutation.isPending}
            className="px-4 py-2 bg-pink-500 text-white rounded-md hover:bg-pink-600 disabled:opacity-50"
          >
            {searchMutation.isPending ? "Searching..." : "Search"}
          </button>
          <button 
            onClick={handleReset}
            className="px-4 py-2 bg-gray-500 text-white rounded-md hover:bg-gray-600"
          >
            Reset
          </button>
        </div>
      </div>

      {/* Action Buttons */}
      <div className="mb-4 flex justify-end gap-4">
        <button onClick={handleAdd} className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600">
          Add New
        </button>
        <button onClick={handleEdit} className="px-4 py-2 bg-yellow-500 text-white rounded-md hover:bg-yellow-600">
          Edit
        </button>
        <button onClick={handleDelete} className="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600">
          Delete
        </button>
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
            </tr>
          </thead>
          <tbody>
            {personals.length > 0 ? (
              personals.map((p) => (
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
                    <Tag color={p.roleName === "ADMIN" ? "red" : p.roleName === "STAFF" ? "blue" : "green"}>
                      {p.roleName}
                    </Tag>
                  </td>
                  <td className="p-3 text-center">
                    <Tag color={p.statusVerify ? "green" : "orange"}>
                      {p.statusVerify ? "Verified" : "Pending"}
                    </Tag>
                  </td>
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
