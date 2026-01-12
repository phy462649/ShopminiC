import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { Form, Input, Button, Card, message } from "antd";
import { LockOutlined, SafetyOutlined } from "@ant-design/icons";
import { authService } from "../../../../services";

const ResetPassword = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [email, setEmail] = useState("");
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [resending, setResending] = useState(false);

  useEffect(() => {
    const stateEmail = location.state?.email;
    const storedEmail = sessionStorage.getItem("resetPasswordEmail");
    const foundEmail = stateEmail || storedEmail || "";
    
    if (foundEmail) {
      setEmail(foundEmail);
    } else {
      navigate("/forgot-password");
    }
  }, [location.state, navigate]);

  const handleResendOTP = async () => {
    setResending(true);
    try {
      await authService.requestPasswordReset(email);
      message.success("Đã gửi lại mã OTP đến email!");
    } catch (error) {
      message.error(error?.message || "Không thể gửi lại mã OTP!");
    } finally {
      setResending(false);
    }
  };

  const handleSubmit = async (values) => {
    setLoading(true);
    try {
      const response = await authService.resetPassword({
        email,
        otp: values.otp,
        newPassword: values.newPassword,
        newPasswordVerify: values.confirmPassword,
      });
      
      if (response.status) {
        message.success("Đặt lại mật khẩu thành công!");
        sessionStorage.removeItem("resetPasswordEmail");
        navigate("/login");
      } else {
        message.error(response.message || "Đặt lại mật khẩu thất bại!");
      }
    } catch (error) {
      message.error(error.message || "Có lỗi xảy ra!");
    } finally {
      setLoading(false);
    }
  };

  if (!email) {
    return <div className="min-h-screen flex items-center justify-center">Đang tải...</div>;
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-100 to-purple-100 p-4">
      <Card className="w-full max-w-md shadow-xl">
        <div className="text-center mb-6">
          <h1 className="text-2xl font-bold text-gray-800">Đặt lại mật khẩu</h1>
          <p className="text-gray-500 mt-1">Nhập mã OTP đã gửi đến email: {email}</p>
        </div>

        <Form
          form={form}
          name="reset-password"
          onFinish={handleSubmit}
          layout="vertical"
          size="large"
        >
          <Form.Item
            name="otp"
            label="Mã OTP"
            rules={[
              { required: true, message: "Vui lòng nhập mã OTP!" },
              { len: 6, message: "Mã OTP phải có 6 ký tự!" },
            ]}
          >
            <Input
              prefix={<SafetyOutlined className="text-gray-400" />}
              placeholder="Nhập mã OTP 6 số"
              maxLength={6}
            />
          </Form.Item>

          <Form.Item
            name="newPassword"
            label="Mật khẩu mới"
            rules={[
              { required: true, message: "Vui lòng nhập mật khẩu mới!" },
              { min: 6, message: "Mật khẩu phải có ít nhất 6 ký tự!" },
              {
                pattern: /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$/,
                message: "Mật khẩu phải có chữ hoa, chữ thường và số!",
              },
            ]}
          >
            <Input.Password
              prefix={<LockOutlined className="text-gray-400" />}
              placeholder="Nhập mật khẩu mới"
            />
          </Form.Item>

          <Form.Item
            name="confirmPassword"
            label="Xác nhận mật khẩu"
            dependencies={["newPassword"]}
            rules={[
              { required: true, message: "Vui lòng xác nhận mật khẩu!" },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue("newPassword") === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error("Mật khẩu không khớp!"));
                },
              }),
            ]}
          >
            <Input.Password
              prefix={<LockOutlined className="text-gray-400" />}
              placeholder="Nhập lại mật khẩu mới"
            />
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              className="w-full h-12 bg-indigo-600 hover:bg-indigo-700"
            >
              Đặt lại mật khẩu
            </Button>
          </Form.Item>
        </Form>

        <div className="text-center">
          <button
            type="button"
            onClick={handleResendOTP}
            disabled={resending}
            className="text-indigo-600 font-medium hover:underline disabled:opacity-50"
          >
            {resending ? "Đang gửi..." : "Gửi lại mã OTP"}
          </button>
        </div>
      </Card>
    </div>
  );
};

export default ResetPassword;
