import { Tag } from 'antd';

const STATUS_CONFIG = {
  // Booking statuses
  pending: { color: 'orange', text: 'Chờ xử lý' },
  confirmed: { color: 'blue', text: 'Đã xác nhận' },
  completed: { color: 'green', text: 'Hoàn thành' },
  cancelled: { color: 'red', text: 'Đã hủy' },
  
  // Order statuses
  shipped: { color: 'cyan', text: 'Đang giao' },
  delivered: { color: 'green', text: 'Đã giao' },
  
  // Payment statuses
  paid: { color: 'green', text: 'Đã thanh toán' },
  unpaid: { color: 'red', text: 'Chưa thanh toán' },
  refunded: { color: 'purple', text: 'Đã hoàn tiền' },
  
  // General
  active: { color: 'green', text: 'Hoạt động' },
  inactive: { color: 'default', text: 'Không hoạt động' },
};

export default function StatusTag({ status, customConfig }) {
  const config = customConfig || STATUS_CONFIG[status?.toLowerCase()] || {
    color: 'default',
    text: status,
  };

  return <Tag color={config.color}>{config.text}</Tag>;
}

export { STATUS_CONFIG };
