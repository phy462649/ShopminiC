import { useState, useEffect, useMemo } from "react";
import { message, Spin } from "antd";
import { usePersonal } from "../../Personal/hooks/usePersonal";
import { useStaff } from "../../Staff/hooks/useStaff";
import { useRoom } from "../../Room/hooks/useRoom";
import { useService } from "../../Services/hooks/useService";
import { useBooking, checkStaffAvailability } from "../hooks/useBooking";

export default function BookingForm({ initialData, onClose, onSave, isLoading }) {
  const { data: customers = [] } = usePersonal();
  const { data: staffList = [] } = useStaff();
  const { data: rooms = [] } = useRoom();
  const { data: services = [] } = useService();
  const { data: allBookings = [] } = useBooking();

  const isEditing = !!initialData?.id;
  const isPending = initialData?.status === 0 || initialData?.status === 'pending';
  const canEditServices = !isEditing || isPending;

  const formatDateTimeLocal = (dateStr) => {
    const date = new Date(dateStr);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  };

  const [formData, setFormData] = useState(() => ({
    customerId: initialData?.customerId || "",
    staffId: initialData?.staffId || "",
    roomId: initialData?.roomId || "",
    startTime: initialData?.startTime ? formatDateTimeLocal(initialData.startTime) : "",
    endTime: initialData?.endTime ? formatDateTimeLocal(initialData.endTime) : "",
    status: initialData?.status ?? 0,
    note: initialData?.note || "",
  }));

  // Selected services list
  const [selectedServices, setSelectedServices] = useState(() => {
    if (initialData?.services?.length > 0) {
      return initialData.services.map(s => ({
        serviceId: s.serviceId,
        serviceName: s.serviceName,
        price: s.price,
        quantity: s.quantity,
      }));
    }
    return [];
  });

  const [currentServiceId, setCurrentServiceId] = useState("");
  const [currentQuantity, setCurrentQuantity] = useState(1);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleAddService = () => {
    if (!currentServiceId) {
      return message.warning("Please select a service");
    }
    
    const service = services.find(s => s.id === Number(currentServiceId));
    if (!service) return;

    // Check if already added
    const exists = selectedServices.find(s => s.serviceId === service.id);
    if (exists) {
      return message.warning("Service already added");
    }

    setSelectedServices(prev => [...prev, {
      serviceId: service.id,
      serviceName: service.name,
      price: service.price,
      quantity: currentQuantity,
    }]);

    setCurrentServiceId("");
    setCurrentQuantity(1);
  };

  const handleRemoveService = (serviceId) => {
    setSelectedServices(prev => prev.filter(s => s.serviceId !== serviceId));
  };

  const handleQuantityChange = (serviceId, quantity) => {
    if (quantity < 1) return;
    setSelectedServices(prev => prev.map(s => 
      s.serviceId === serviceId ? { ...s, quantity } : s
    ));
  };

  const totalAmount = selectedServices.reduce((sum, s) => sum + (s.price * s.quantity), 0);

  const handleSubmit = (e) => {
    e.preventDefault();
    
    if (!formData.customerId || !formData.staffId || !formData.roomId) {
      return message.warning("Please select customer, staff and room");
    }
    if (!formData.startTime) {
      return message.warning("Please select start time");
    }
    if (selectedServices.length === 0) {
      return message.warning("Please add at least one service");
    }

    onSave({
      ...formData,
      customerId: Number(formData.customerId),
      staffId: Number(formData.staffId),
      roomId: Number(formData.roomId),
      status: Number(formData.status),
      services: selectedServices.map(s => ({
        serviceId: s.serviceId,
        quantity: s.quantity,
      })),
    });
  };

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          <h2 className="text-xl font-bold text-gray-800 mb-4">
            {isEditing ? "Edit Booking" : "Add New Booking"}
          </h2>

          <form onSubmit={handleSubmit} className="space-y-4">
            {/* Customer */}
            <div>
              <label className="block text-sm font-medium mb-1">Customer *</label>
              {isEditing ? (
                <input
                  type="text"
                  value={initialData?.customerName || `Customer #${formData.customerId}`}
                  disabled
                  className="w-full border rounded-md px-3 py-2 bg-gray-100 cursor-not-allowed"
                />
              ) : (
                <select
                  name="customerId"
                  value={formData.customerId}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                >
                  <option value="">-- Select Customer --</option>
                  {customers.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.name} - {c.phone}
                    </option>
                  ))}
                </select>
              )}
            </div>

            <div className="grid grid-cols-2 gap-4">
              {/* Staff */}
              <div>
                <label className="block text-sm font-medium mb-1">Staff *</label>
                {isEditing ? (
                  <input
                    type="text"
                    value={initialData?.staffName || `Staff #${formData.staffId}`}
                    disabled
                    className="w-full border rounded-md px-3 py-2 bg-gray-100 cursor-not-allowed"
                  />
                ) : (
                  <select
                    name="staffId"
                    value={formData.staffId}
                    onChange={handleChange}
                    className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                  >
                    <option value="">-- Select Staff --</option>
                    {staffList.map((s) => (
                      <option key={s.id} value={s.id}>
                        {s.name}
                      </option>
                    ))}
                  </select>
                )}
              </div>

              {/* Room */}
              <div>
                <label className="block text-sm font-medium mb-1">Room *</label>
                {isEditing ? (
                  <input
                    type="text"
                    value={initialData?.roomName || `Room #${formData.roomId}`}
                    disabled
                    className="w-full border rounded-md px-3 py-2 bg-gray-100 cursor-not-allowed"
                  />
                ) : (
                  <select
                    name="roomId"
                    value={formData.roomId}
                    onChange={handleChange}
                    className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                  >
                    <option value="">-- Select Room --</option>
                    {rooms.map((r) => (
                      <option key={r.id} value={r.id}>
                        {r.name}
                      </option>
                    ))}
                  </select>
                )}
              </div>
            </div>

            {/* Services */}
            <div>
              <label className="block text-sm font-medium mb-1">Services *</label>
              
              {/* Add Service Row - Only show if can edit */}
              {canEditServices && (
                <div className="flex gap-2 mb-2">
                  <select
                    value={currentServiceId}
                    onChange={(e) => setCurrentServiceId(e.target.value)}
                    className="flex-1 border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                  >
                    <option value="">-- Select Service --</option>
                    {services.map((s) => (
                      <option key={s.id} value={s.id}>
                        {s.name} - ${s.price?.toLocaleString("en-US")}
                      </option>
                    ))}
                  </select>
                  <input
                    type="number"
                    min="1"
                    value={currentQuantity}
                    onChange={(e) => setCurrentQuantity(Number(e.target.value))}
                    className="w-20 border rounded-md px-3 py-2 text-center focus:ring-2 focus:ring-pink-500"
                  />
                  <button
                    type="button"
                    onClick={handleAddService}
                    className="px-4 py-2 bg-pink-500 text-white rounded-md hover:bg-pink-600"
                  >
                    + Add
                  </button>
                </div>
              )}

              {/* Selected Services List */}
              {selectedServices.length > 0 && (
                <div className="border rounded-md overflow-hidden">
                  <table className="w-full text-sm">
                    <thead className="bg-gray-100">
                      <tr>
                        <th className="text-left p-2">Service</th>
                        <th className="text-center p-2 w-24">Qty</th>
                        <th className="text-right p-2">Price</th>
                        <th className="text-right p-2">Subtotal</th>
                        {canEditServices && <th className="text-center p-2 w-16">Action</th>}
                      </tr>
                    </thead>
                    <tbody>
                      {selectedServices.map((s) => (
                        <tr key={s.serviceId} className="border-t">
                          <td className="p-2">{s.serviceName}</td>
                          <td className="p-2 text-center">
                            {canEditServices ? (
                              <input
                                type="number"
                                min="1"
                                value={s.quantity}
                                onChange={(e) => handleQuantityChange(s.serviceId, Number(e.target.value))}
                                className="w-16 border rounded px-2 py-1 text-center"
                              />
                            ) : (
                              s.quantity
                            )}
                          </td>
                          <td className="p-2 text-right">${s.price?.toLocaleString("en-US")}</td>
                          <td className="p-2 text-right font-medium text-pink-600">
                            ${(s.price * s.quantity)?.toLocaleString("en-US")}
                          </td>
                          {canEditServices && (
                            <td className="p-2 text-center">
                              <button
                                type="button"
                                onClick={() => handleRemoveService(s.serviceId)}
                                className="text-red-500 hover:text-red-700"
                              >
                                ✕
                              </button>
                            </td>
                          )}
                        </tr>
                      ))}
                    </tbody>
                    <tfoot className="bg-pink-50">
                      <tr className="font-semibold">
                        <td colSpan="3" className="p-2 text-right">Total:</td>
                        <td className="p-2 text-right text-pink-600">${totalAmount?.toLocaleString("en-US")}</td>
                        {canEditServices && <td></td>}
                      </tr>
                    </tfoot>
                  </table>
                </div>
              )}
            </div>

            {/* Time */}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1">Start Time *</label>
                <input
                  type="datetime-local"
                  name="startTime"
                  value={formData.startTime}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">End Time</label>
                <input
                  type="datetime-local"
                  name="endTime"
                  value={formData.endTime}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                />
              </div>
            </div>

            {/* Status */}
            <div>
              <label className="block text-sm font-medium mb-1">Status</label>
              {isEditing ? (
                <input
                  type="text"
                  value={formData.status === 0 ? 'Pending' : formData.status === 1 ? 'Confirmed' : formData.status === 2 ? 'Completed' : 'Cancelled'}
                  disabled
                  className="w-full border rounded-md px-3 py-2 bg-gray-100 cursor-not-allowed"
                />
              ) : (
                <select
                  name="status"
                  value={formData.status}
                  onChange={handleChange}
                  className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                >
                  <option value={0}>Pending</option>
                 
                </select>
              )}
            </div>

            {/* Note */}
            <div>
              <label className="block text-sm font-medium mb-1">Note</label>
              <textarea
                name="note"
                value={formData.note}
                onChange={handleChange}
                rows={2}
                className="w-full border rounded-md px-3 py-2 focus:ring-2 focus:ring-pink-500"
                placeholder="Additional notes..."
              />
            </div>

            {/* Actions */}
            <div className="flex justify-end gap-3 pt-4 border-t">
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 border rounded-md hover:bg-gray-100"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isLoading}
                className="px-4 py-2 bg-pink-500 text-white rounded-md hover:bg-pink-600 disabled:opacity-50 flex items-center gap-2"
              >
                {isLoading && <Spin size="small" />}
                {isEditing ? "Update" : "Create"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
