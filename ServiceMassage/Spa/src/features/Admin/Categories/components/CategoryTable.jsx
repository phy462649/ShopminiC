import { useState } from "react";
import { message, Tag, Modal } from "antd";
import CategoryForm from "./CategoryForm";
import {
  useCategory,
  useCreateCategory,
  useUpdateCategory,
  useDeleteCategory,
} from "../hooks/useCategory";

export default function CategoryTable() {
  const { data: categories = [], isLoading, isError } = useCategory();
  const createMutation = useCreateCategory();
  const updateMutation = useUpdateCategory();
  const deleteMutation = useDeleteCategory();

  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  const filteredCategories = categories.filter((c) =>
    [c.name, c.description]
      .filter(Boolean)
      .some((field) => field.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const handleAdd = () => {
    setEditingCategory(null);
    setFormOpen(true);
  };

  const handleEdit = () => {
    if (!selectedId) return message.warning("Please select a category to edit");
    const category = categories.find((c) => c.id === selectedId);
    if (category) {
      setEditingCategory(category);
      setFormOpen(true);
    }
  };

  const handleDelete = () => {
    if (!selectedId) return message.warning("Please select a category to delete");
    Modal.confirm({
      title: "Confirm Delete",
      content: "Are you sure you want to delete this category?",
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
    if (editingCategory) {
      updateMutation.mutate(
        { id: editingCategory.id, data },
        { onSuccess: () => setFormOpen(false) }
      );
    } else {
      createMutation.mutate(data, {
        onSuccess: () => setFormOpen(false),
      });
    }
  };

  // const getParentName = (parentId) => {
  //   const parent = categories.find((c) => c.id === parentId);
  //   return parent?.name || "-";
  // };

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
              <th className="p-3 text-center font-semibold">Description</th>
              {/* <th className="p-3 text-center font-semibold">Parent</th> */}
              <th className="p-3 text-center font-semibold">Status</th>
              {/* <th className="p-3 text-center font-semibold">Created At</th> */}
            </tr>
          </thead>
          <tbody>
            {filteredCategories.length > 0 ? (
              filteredCategories.map((c) => (
                <tr
                  key={c.id}
                  onClick={() => setSelectedId(c.id)}
                  className={`cursor-pointer border-b hover:bg-gray-50 ${selectedId === c.id ? "bg-pink-100" : ""}`}
                >
                  <td className="p-3 text-center font-medium">{c.id}</td>
                  <td className="p-3 text-center">{c.name}</td>
                  <td className="p-3 text-center">{c.description || "-"}</td>
                  {/* <td className="p-3 text-center">{getParentName(c.parent_id)}</td> */}
                  <td className="p-3 text-center">
                    <Tag color={c.status ? "green" : "red"}>
                      {c.status ? "Active" : "Inactive"}
                    </Tag>
                  </td>
                  {/* <td className="p-3 text-center">
                    {c.created_at ? new Date(c.created_at).toLocaleDateString("en-US") : "-"}
                  </td> */}
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

      {/* Form Modal */}
      {formOpen && (
        <CategoryForm
          key={editingCategory?.id || 'new'}
          initialData={editingCategory}
          categories={categories}
          onClose={() => setFormOpen(false)}
          onSave={handleSave}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      )}
    </div>
  );
}
