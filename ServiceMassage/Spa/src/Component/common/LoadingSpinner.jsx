import { Spin } from 'antd';

export default function LoadingSpinner({ size = 'large', tip = 'Đang tải...' }) {
  return (
    <div className="flex items-center justify-center min-h-[200px]">
      <Spin size={size} tip={tip} />
    </div>
  );
}
