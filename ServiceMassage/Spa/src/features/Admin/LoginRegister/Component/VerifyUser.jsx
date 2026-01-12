import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { Form, Input, Button, Card, message, Result } from "antd";
import { MailOutlined, SafetyOutlined } from "@ant-design/icons";
import { authService } from "../../../../services";

const VerifyUser = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [resendLoading, setResendLoading] = useState(false);
  const [countdown, setCountdown] = useState(0);

  // Lấy email từ route state
  const email = location.state?.email;

  // Countdown cho nút gửi lại
  useEffect(() => {
    if (countdown > 0) {
      const timer = setTimeout(() => setCountdown(countdown - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [countdown]);

  // Nếu không có email, hiển thị thông báo lỗi
  if (!email) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-100 to-purple-100 p-4">
        <Card className="w-full max-w-md shadow-xl">
          <Result
            status="warning"
            title="Không tìm thấy email xác nhận"
            subTitle="Vui lòng đăng ký lại để nhận mã xác nhận."
            extra={[
              <Button
                type="primary"
                key="register"
                onClick={() => navigate("/register")}
                className="bg-indigo-600 hover:bg-indigo-700"
              >
                Đăng ký
              </Button>,
              <Button key="login" onClick={() => navigate("/login")}>
                Đăng nhập
              </Button>,
            ]}
          />
        </Card>
      </div>
    );
  }

  const handleSubmit = async (values) => {
    setLoading(true);
    try {
      // Gọi API xác nhận email - backend cần email và otp
      const response = await authService.verifyEmail({
        email: email,
        otp: values.otp,
      });

      if (response.status) {
        message.success(response.message || "Xác nhận email thành công!");
        navigate("/login");
      } else {
        message.error(response.message || "Mã xác nhận không đúng!");
      }
    } catch (error) {
      console.error("Verify error:", error);
      const errorMsg = error.response?.data?.message || error.message || "Xác nhận thất bại!";
      message.error(errorMsg);
    } finally {
      setLoading(false);
    }
  };

  const handleResend = async () => {
    setResendLoading(true);
    try {
      const response = await authService.resendVerification(email);
      
      if (response.status) {
        message.success(response.message || "Đã gửi lại mã xác nhận!");
        setCountdown(60); // Đợi 60 giây trước khi gửi lại
      } else {
        message.error(response.message || "Không thể gửi lại mã!");
      }
    } catch (error) {
      console.error("Resend error:", error);
      const errorMsg = error.response?.data?.message || error.message || "Không thể gửi lại mã!";
      message.error(errorMsg);
    } finally {
      setResendLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-100 to-purple-100 p-4">
      <Card className="w-full max-w-md shadow-xl">
        <div className="text-center mb-6">
          <div className="w-16 h-16 bg-indigo-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <MailOutlined className="text-3xl text-indigo-600" />
          </div>
          <h1 className="text-2xl font-bold text-gray-800">Xác nhận email</h1>
          <p className="text-gray-500 mt-2">
            Mã xác nhận đã được gửi tới email
          </p>
          <p className="text-indigo-600 font-medium">{email}</p>
        </div>

        <Form
          form={form}
          name="verify-email"
          onFinish={handleSubmit}
          layout="vertical"
          size="large"
        >
          <Form.Item
            name="otp"
            label="Mã xác nhận"
            rules={[
              { required: true, message: "Vui lòng nhập mã xác nhận!" },
              { len: 6, message: "Mã xác nhận gồm 6 chữ số!" },
              { pattern: /^\d+$/, message: "Mã xác nhận chỉ chứa số!" },
            ]}
          >
            <Input
              prefix={<SafetyOutlined className="text-gray-400" />}
              placeholder="Nhập mã 6 số"
              maxLength={6}
              className="text-center text-lg tracking-widest"
            />
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              className="w-full h-12 bg-indigo-600 hover:bg-indigo-700"
            >
              Xác nhận
            </Button>
          </Form.Item>
        </Form>

        <div className="text-center space-y-3">
          <p className="text-gray-600">
            Không nhận được mã?{" "}
            <button
              type="button"
              onClick={handleResend}
              disabled={countdown > 0 || resendLoading}
              className={`font-medium ${
                countdown > 0
                  ? "text-gray-400 cursor-not-allowed"
                  : "text-indigo-600 hover:underline cursor-pointer"
              }`}
            >
              {resendLoading
                ? "Đang gửi..."
                : countdown > 0
                ? `Gửi lại sau ${countdown}s`
                : "Gửi lại"}
            </button>
          </p>
          <p className="text-gray-600">
            <button
              type="button"
              onClick={() => navigate("/login")}
              className="text-indigo-600 font-medium hover:underline"
            >
              Quay lại đăng nhập
            </button>
          </p>
        </div>
      </Card>
    </div>
  );
};

export default VerifyUser;
