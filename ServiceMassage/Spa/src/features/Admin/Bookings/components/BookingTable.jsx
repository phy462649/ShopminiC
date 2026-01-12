import { useQuery } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import { message } from "antd";
import BookingForm from "./BookingForm";
import { 
  useBookings, 
  useCreateBooking, 
  useUpdateBooking, 
  useDeleteBooking 
} from "../../../../../Hooks/useBookings";

export default function BookingTable() {
  const { data, isLoading, isError } = useBookings();
  const createBookingMutation = useCreateBooking();
  const updateBookingMutation = useUpdateBooking();
  const deleteBookingMutation = useDeleteBooking();

  const [bookingList, setBookingList] = useState([]);
  const [selectedId, setSelectedId] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingBooking, setEditingBooking] = useState(null);

  const emptyBooking = {
    customer_id: "",
    staff_id: "",
    room_id: "",
    start_time: "",
    end_time: "",
    status: "pending",
  };

  useEffect(() => {
    // When data from API changes, update local list (or just renders directly from data if pagination not needed locally)
    // For now, keep local state sync pattern if filtering happens locally, 
    // but ideally we should render 'data' directly.
    if (data) setBookingList(data);
  }, [data]);

  const handleAdd = () => {
    setEditingBooking(null);
    setFormOpen(true);
  };

  const handleUpdate = () => {
    if (!selectedId) return message.warning("Chọn booking để sửa");

    const booking = bookingList.find((b) => b.id === selectedId);
    if (!booking) return;

    setEditingBooking(booking);
    setFormOpen(true);
  };

  const handleDelete = () => {
    if (!selectedId) return message.error("Chọn booking để xoá");

    if (window.confirm("Bạn có chắc chắn muốn xóa booking này?")) {
      deleteBookingMutation.mutate(selectedId, {
        onSuccess: () => {
          setSelectedId(null);
        }
      });
    }
  };

  const handleCloseForm = () => setFormOpen(false);

  const handleSave = (newBooking) => {
    if (editingBooking) {
      // update
      updateBookingMutation.mutate({ 
        id: editingBooking.id, 
        data: newBooking 
      }, {
        onSuccess: () => {
          setFormOpen(false);
          setEditingBooking(null);
        }
      });
    } else {
      // add new
      createBookingMutation.mutate(newBooking, {
        onSuccess: () => {
          setFormOpen(false);
          setEditingBooking(null);
        }
      });
    }
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
            placeholder="Tìm kiếm booking..."
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
              <th className="p-2 border">Khách</th>
              <th className="p-2 border">Nhân viên</th>
              <th className="p-2 border">Phòng</th>
              <th className="p-2 border">Bắt đầu</th>
              <th className="p-2 border">Kết thúc</th>
              <th className="p-2 border">Trạng thái</th>
            </tr>
          </thead>

          <tbody>
            {(bookingList && bookingList.length > 0) ? (
              bookingList.map((b) => (
                <tr
                  key={b.id}
                  className={`cursor-pointer hover:bg-gray-50 ${
                    selectedId === b.id ? "bg-pink-100" : ""
                  }`}
                  onClick={() => setSelectedId(b.id)}
                >
                  <td className="p-2 border text-center">{b.id}</td>
                  <td className="p-2 border text-center">{b.customer_id}</td>
                  <td className="p-2 border text-center">{b.staff_id}</td>
                  <td className="p-2 border text-center">{b.room_id}</td>
                  <td className="p-2 border text-center">
                    {new Date(b.start_time).toLocaleString("vi-VN")}
                  </td>
                  <td className="p-2 border text-center">
                    {b.end_time
                      ? new Date(b.end_time).toLocaleString("vi-VN")
                      : "-"}
                  </td>
                  <td className="p-2 border text-center">{b.status}</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="7" className="p-4 text-center text-gray-500">
                  Không có dữ liệu
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Form Add / Edit */}
      {formOpen && (
        <BookingForm
          initialData={editingBooking || emptyBooking}
          onClose={handleCloseForm}
          onSave={handleSave}
        />
      )}
    </div>
  );
}
