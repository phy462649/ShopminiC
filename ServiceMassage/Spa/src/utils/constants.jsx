// Booking statuses
export const BOOKING_STATUS = {
  PENDING: 'pending',
  CONFIRMED: 'confirmed',
  COMPLETED: 'completed',
  CANCELLED: 'cancelled',
};

export const BOOKING_STATUS_OPTIONS = [
  { value: BOOKING_STATUS.PENDING, label: 'Chờ xử lý', color: 'orange' },
  { value: BOOKING_STATUS.CONFIRMED, label: 'Đã xác nhận', color: 'blue' },
  { value: BOOKING_STATUS.COMPLETED, label: 'Hoàn thành', color: 'green' },
  { value: BOOKING_STATUS.CANCELLED, label: 'Đã hủy', color: 'red' },
];

// Order statuses
export const ORDER_STATUS = {
  PENDING: 'pending',
  CONFIRMED: 'confirmed',
  SHIPPED: 'shipped',
  COMPLETED: 'completed',
  CANCELLED: 'cancelled',
};

export const ORDER_STATUS_OPTIONS = [
  { value: ORDER_STATUS.PENDING, label: 'Chờ xử lý', color: 'orange' },
  { value: ORDER_STATUS.CONFIRMED, label: 'Đã xác nhận', color: 'blue' },
  { value: ORDER_STATUS.SHIPPED, label: 'Đang giao', color: 'cyan' },
  { value: ORDER_STATUS.COMPLETED, label: 'Hoàn thành', color: 'green' },
  { value: ORDER_STATUS.CANCELLED, label: 'Đã hủy', color: 'red' },
];

// Payment statuses
export const PAYMENT_STATUS = {
  PENDING: 'pending',
  COMPLETED: 'completed',
  FAILED: 'failed',
  REFUNDED: 'refunded',
};

export const PAYMENT_STATUS_OPTIONS = [
  { value: PAYMENT_STATUS.PENDING, label: 'Chờ thanh toán', color: 'orange' },
  { value: PAYMENT_STATUS.COMPLETED, label: 'Đã thanh toán', color: 'green' },
  { value: PAYMENT_STATUS.FAILED, label: 'Thất bại', color: 'red' },
  { value: PAYMENT_STATUS.REFUNDED, label: 'Đã hoàn tiền', color: 'purple' },
];

// Payment methods
export const PAYMENT_METHOD = {
  CASH: 'cash',
  CARD: 'card',
  TRANSFER: 'transfer',
  MOMO: 'momo',
  VNPAY: 'vnpay',
};

export const PAYMENT_METHOD_OPTIONS = [
  { value: PAYMENT_METHOD.CASH, label: 'Tiền mặt' },
  { value: PAYMENT_METHOD.CARD, label: 'Thẻ' },
  { value: PAYMENT_METHOD.TRANSFER, label: 'Chuyển khoản' },
  { value: PAYMENT_METHOD.MOMO, label: 'MoMo' },
  { value: PAYMENT_METHOD.VNPAY, label: 'VNPay' },
];

// Days of week
export const DAYS_OF_WEEK = [
  { value: 0, label: 'Chủ nhật' },
  { value: 1, label: 'Thứ hai' },
  { value: 2, label: 'Thứ ba' },
  { value: 3, label: 'Thứ tư' },
  { value: 4, label: 'Thứ năm' },
  { value: 5, label: 'Thứ sáu' },
  { value: 6, label: 'Thứ bảy' },
];

// Pagination defaults
export const PAGINATION = {
  DEFAULT_PAGE: 1,
  DEFAULT_PAGE_SIZE: 10,
  PAGE_SIZE_OPTIONS: ['10', '20', '50', '100'],
};
