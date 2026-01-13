import { useState } from "react";
import { message, Tag, Popconfirm, Tooltip } from "antd";
import BookingForm from "./BookingForm";
import {
  useBooking,
  useCreateBooking,
  useUpdateBooking,
  useDeleteBooking,
  useUpdateBookingStatus,
} from "../hooks/useBooking";

const statusMap = {
  0: { label: "Pending", color: "orange" },
  1: { label: "Confirmed", color: "blue" },
  2: { label: "Completed", color: "green" },
  3: { label: "Cancelled", color: "red" },
  pending: { label: "Pending", color: "orange" },
  confirmed: { label: "Confirmed", color: "blue" },
  completed: { label: "Completed", color: "green" },
  cancelled: { label: "Cancelled", color: "red" },
};

export default function BookingTable() {
  const { data: bookings = [], isLoading, isError } = useBooking();
  const createMutation = useCreateBooking();
  const updateMutation = useUpdateBooking();
  const deleteMutation = useDeleteBooking();
  const updateStatusMutation = useUpdateBookingStatus();

  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingBooking, setEditingBooking] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [expandedId, setExpandedId] = useState(null);

  const filteredBookings = bookings.filter((b) =>
    [b.customerName, b.staffName, b.roomName, String(b.status)]
      .filter(Boolean)
      .some((field) => field.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const handleAdd = () => {
    setEditingBooking(null);
    setFormOpen(true);
  };

  const handleEdit = () => {
    if (!selectedId) return message.warning("Please select a booking to edit");
    const booking = bookings.find((b) => b.id === selectedId);
    if (booking) {
      setEditingBooking(booking);
      setFormOpen(true);
    }
  };

  // const handleDelete = () => {
  //   if (!selectedId) return message.warning("Please select a booking to delete");
  //   deleteMutation.mutate(selectedId, {
  //     onSuccess: () => setSelectedId(null),
  //   });
  // };

  const handleSave = (data) => {
    if (editingBooking) {
      updateMutation.mutate(
        { id: editingBooking.id, data },
        { onSuccess: () => setFormOpen(false) }
      );
    } else {
      createMutation.mutate(data, {
        onSuccess: () => setFormOpen(false),
      });
    }
  };

  const handleStatusChange = (id, status) => {
    updateStatusMutation.mutate({ id, status: Number(status) });
  };

  const toggleExpand = (id) => {
    setExpandedId(expandedId === id ? null : id);
  };

  const getStatusInfo = (status) => statusMap[status] || { label: status, color: "default" };

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
          <button onClick={handleAdd} className="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-pink-600">
             Add
          </button>
          <button onClick={handleEdit} className="px-4 py-2 bg-yellow-500 text-white rounded-md hover:bg-yellow-600">
            Edit
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="overflow-x-auto bg-white rounded-lg shadow">
        <table className="min-w-full text-sm">
          <thead className="bg-pink-50">
            <tr>
              <th className="p-3 text-center font-semibold w-8"></th>
              <th className="p-3 text-center font-semibold">ID</th>
              <th className="p-3 text-center font-semibold">Customer</th>
              <th className="p-3 text-center font-semibold">Staff</th>
              <th className="p-3 text-center font-semibold">Room</th>
              <th className="p-3 text-center font-semibold">Start Time</th>
              <th className="p-3 text-center font-semibold">End Time</th>
              <th className="p-3 text-center font-semibold">Total</th>
              <th className="p-3 text-center font-semibold">Status</th>
            </tr>
          </thead>
          <tbody>
            {filteredBookings.length > 0 ? (
              filteredBookings.map((b) => (
                <>
                  <tr
                    key={b.id}
                    onClick={() => setSelectedId(b.id)}
                    className={`cursor-pointer border-b hover:bg-gray-50 ${selectedId === b.id ? "bg-pink-100" : ""}`}
                  >
                    <td className="p-3 text-center">
                      <button
                        onClick={(e) => { e.stopPropagation(); toggleExpand(b.id); }}
                        className="text-gray-500 hover:text-pink-500"
                      >
                        {expandedId === b.id ? "▼" : "▶"}
                      </button>
                    </td>
                    <td className="p-3 text-center font-medium">{b.id}</td>
                    <td className="p-3 text-center">
                      <div className="font-medium">{b.customerName}</div>
                    </td>
                    <td className="p-3 text-center">
                      <div className="font-medium">{b.staffName}</div>
                    </td>
                    <td className="p-3 text-center">
                      <div className="font-medium">{b.roomName}</div>
                    </td>
                    <td className="p-3 text-center">
                      {b.startTime ? new Date(b.startTime).toLocaleString("en-US") : "-"}
                    </td>
                    <td className="p-3 text-center">
                      {b.endTime ? new Date(b.endTime).toLocaleString("en-US") : "-"}
                    </td>
                    <td className="p-3 text-center font-semibold text-pink-600">
                      ${b.totalAmount?.toLocaleString("en-US")}
                    </td>
                    <td className="p-3 text-center">
                      <Tag color={getStatusInfo(b.status).color}>
                        {getStatusInfo(b.status).label}
                      </Tag>
                    </td>
                  </tr>
                  {/* Expanded Row - Services */}
                  {expandedId === b.id && b.services && b.services.length > 0 && (
                    <tr key={`${b.id}-services`} className="bg-gray-50">
                      <td colSpan="11" className="p-4">
                        <div className="ml-8">
                          <h4 className="font-semibold mb-2 text-gray-700">Booked Services:</h4>
                          <table className="w-full text-sm border">
                            <thead className="bg-gray-100">
                              <tr>
                                <th className="p-2 text-center border">ID</th>
                                <th className="p-2 text-center border">Service Name</th>
                                <th className="p-2 text-center border">Unit Price</th>
                                <th className="p-2 text-center border">Quantity</th>
                                <th className="p-2 text-center border">Subtotal</th>
                              </tr>
                            </thead>
                            <tbody>
                              {b.services.map((service, idx) => (
                                <tr key={service.id} className="border-b">
                                  <td className="p-2 border text-center">{idx + 1}</td>
                                  <td className="p-2 border text-center font-medium">{service.serviceName}</td>
                                  <td className="p-2 border text-center">${service.price?.toLocaleString("en-US")}</td>
                                  <td className="p-2 border text-center">{service.quantity}</td>
                                  <td className="p-2 border text-center font-semibold text-pink-600">
                                    ${service.subtotal?.toLocaleString("en-US")}
                                  </td>
                                </tr>
                              ))}
                              <tr className="bg-pink-50 font-semibold">
                                <td colSpan="4" className="p-2 border text-center">Total:</td>
                                <td className="p-2 border text-center text-pink-600">
                                  ${b.totalAmount?.toLocaleString("en-US")}
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                      </td>
                    </tr>
                  )}
                </>
              ))
            ) : (
              <tr>
                <td colSpan="11" className="p-4 text-center text-gray-500">
                  No data available
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Form Modal */}
      {formOpen && (
        <BookingForm
          key={editingBooking?.id || 'new'}
          initialData={editingBooking}
          onClose={() => setFormOpen(false)}
          onSave={handleSave}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      )}
    </div>
  );
}
