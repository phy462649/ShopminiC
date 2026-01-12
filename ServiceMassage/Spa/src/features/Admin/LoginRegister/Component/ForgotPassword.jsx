import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Form, Input, Button, Card, message } from "antd";
import { MailOutlined } from "@ant-design/icons";
import { authService } from "../../../../services";

const ForgotPassword = () => {
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (values) => {
    setLoading(true);
    try {
      const response = await authService.requestPasswordReset(values.email);
      console.log("API Response:", response);
      
      // API thành công (axios đã trả về data)
      message.success("Đã gửi mã OTP đến email. Vui lòng kiểm tra hộp thư!");
      // Lưu email vào sessionStorage để trang reset-password lấy
      sessionStorage.setItem("resetPasswordEmail", values.email);
      navigate("/reset-password");
    } catch (error) {
      console.error("Error:", error);
      message.error(error?.message || "Có lỗi xảy ra, vui lòng thử lại!");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-100 to-purple-100 p-4">
      <Card className="w-full max-w-md shadow-xl">
        <div className="text-center mb-6">
          <h1 className="text-2xl font-bold text-gray-800">Quên mật khẩu</h1>
          <p className="text-gray-500 mt-1">Nhập email để nhận link khôi phục mật khẩu</p>
        </div>

        <Form
          form={form}
          name="forgot-password"
          onFinish={handleSubmit}
          layout="vertical"
          size="large"
        >
          <Form.Item
            name="email"
            label="Email"
            rules={[
              { required: true, message: "Vui lòng nhập email!" },
              { type: "email", message: "Email không hợp lệ!" },
            ]}
          >
            <Input
              prefix={<MailOutlined className="text-gray-400" />}
              placeholder="Nhập email đã đăng ký"
            />
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              className="w-full h-12 bg-indigo-600 hover:bg-indigo-700"
            >
              Gửi yêu cầu
            </Button>
          </Form.Item>
        </Form>

        <div className="text-center">
          <button
            type="button"
            onClick={() => navigate("/login")}
            className="text-indigo-600 font-medium hover:underline"
          >
            Quay lại đăng nhập
          </button>
        </div>
      </Card>
    </div>
  );
};

export default ForgotPassword;
