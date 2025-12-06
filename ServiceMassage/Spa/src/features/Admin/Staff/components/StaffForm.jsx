import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

const staffSchema = z.object({
  name: z.string().min(1, "Tên bắt buộc"),
  phone: z.string().optional(),
  email: z.string().email("Email không hợp lệ").optional().or(z.literal("")),
  specialty: z.string().optional(),
});

export default function StaffForm({ onSubmit, loading = false, error = null }) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(staffSchema),
  });

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="space-y-4 max-w-md mx-auto p-4"
      aria-describedby={error ? "form-error" : undefined}
    >
      {error && (
        <p id="form-error" className="text-red-600 text-sm">
          {error}
        </p>
      )}

      <div>
        <label htmlFor="name" className="block text-sm font-medium">
          Tên <span className="text-red-500">*</span>
        </label>
        <input
          id="name"
          {...register("name")}
          className="mt-1 w-full border rounded px-3 py-2"
          aria-invalid={!!errors.name}
        />
        {errors.name && (
          <p className="text-red-600 text-sm">{errors.name.message}</p>
        )}
      </div>

      <div>
        <label htmlFor="phone" className="block text-sm font-medium">
          Số điện thoại
        </label>
        <input
          id="phone"
          {...register("phone")}
          className="mt-1 w-full border rounded px-3 py-2"
        />
      </div>

      <div>
        <label htmlFor="email" className="block text-sm font-medium">
          Email
        </label>
        <input
          id="email"
          {...register("email")}
          className="mt-1 w-full border rounded px-3 py-2"
          aria-invalid={!!errors.email}
        />
        {errors.email && (
          <p className="text-red-600 text-sm">{errors.email.message}</p>
        )}
      </div>

      <div>
        <label htmlFor="specialty" className="block text-sm font-medium">
          Chuyên môn
        </label>
        <input
          id="specialty"
          {...register("specialty")}
          className="mt-1 w-full border rounded px-3 py-2"
        />
      </div>

      <button
        type="submit"
        disabled={loading}
        className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 disabled:opacity-50"
      >
        {loading ? "Đang lưu..." : "Lưu nhân viên"}
      </button>
    </form>
  );
}
