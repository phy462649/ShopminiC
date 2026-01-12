import { Input } from 'antd';
import { SearchOutlined } from '@ant-design/icons';

export default function SearchInput({
  value,
  onChange,
  placeholder = 'Tìm kiếm...',
  className = '',
  allowClear = true,
}) {
  return (
    <Input
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder}
      prefix={<SearchOutlined className="text-gray-400" />}
      allowClear={allowClear}
      className={`max-w-xs ${className}`}
    />
  );
}
