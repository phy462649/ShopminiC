import { Modal } from 'antd';
import { ExclamationCircleOutlined } from '@ant-design/icons';

const { confirm } = Modal;

export function showConfirm({
  title = 'Xác nhận',
  content = 'Bạn có chắc chắn muốn thực hiện hành động này?',
  okText = 'Đồng ý',
  cancelText = 'Hủy',
  onOk,
  onCancel,
  danger = false,
}) {
  confirm({
    title,
    icon: <ExclamationCircleOutlined />,
    content,
    okText,
    cancelText,
    okButtonProps: { danger },
    onOk,
    onCancel,
  });
}

export function showDeleteConfirm({
  title = 'Xác nhận xóa',
  content = 'Bạn có chắc chắn muốn xóa? Hành động này không thể hoàn tác.',
  onOk,
}) {
  showConfirm({
    title,
    content,
    okText: 'Xóa',
    danger: true,
    onOk,
  });
}

export default { showConfirm, showDeleteConfirm };
