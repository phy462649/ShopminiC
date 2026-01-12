import { Button, Space } from 'antd';
import { PlusOutlined, ReloadOutlined } from '@ant-design/icons';

export default function PageHeader({
  title,
  subtitle,
  onAdd,
  addText = 'Thêm mới',
  onRefresh,
  extra,
}) {
  return (
    <div className="flex items-center justify-between mb-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-800">{title}</h1>
        {subtitle && <p className="text-gray-500 mt-1">{subtitle}</p>}
      </div>
      
      <Space>
        {onRefresh && (
          <Button icon={<ReloadOutlined />} onClick={onRefresh}>
            Làm mới
          </Button>
        )}
        {onAdd && (
          <Button type="primary" icon={<PlusOutlined />} onClick={onAdd}>
            {addText}
          </Button>
        )}
        {extra}
      </Space>
    </div>
  );
}
