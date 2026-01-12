import { Alert, Button } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

export default function ErrorMessage({ 
  message = 'Đã xảy ra lỗi', 
  description,
  onRetry 
}) {
  return (
    <div className="p-4">
      <Alert
        message={message}
        description={description}
        type="error"
        showIcon
        action={
          onRetry && (
            <Button 
              size="small" 
              icon={<ReloadOutlined />}
              onClick={onRetry}
            >
              Thử lại
            </Button>
          )
        }
      />
    </div>
  );
}
