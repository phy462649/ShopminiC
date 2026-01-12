import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Form, Input, Button, Card, message } from "antd";
import { UserOutlined, LockOutlined } from "@ant-design/icons";
import { authService } from "../../../../services";
import { getOrCreateDeviceToken } from "../../../../utils";

const LoginForm = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (values) => {
    setLoading(true);
    try {
      const deviceToken = getOrCreateDeviceToken();
      const response = await authService.login({
        username: values.username,
        password: values.password,
        deviceToken: deviceToken
      });

      if (response.status) {
        message.success(response.message || "Đăng nhập thành công!");
        navigate("/");
      } else {
        message.error(response.message || "Đăng nhập thất bại");
      }
    } catch (error) {
      message.error(error.message || "Đăng nhập thất bại. Vui lòng thử lại.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-100 to-purple-100 p-4">
      <Card className="w-full max-w-md shadow-xl">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-gray-800">Đăng nhập</h1>
          <p className="text-gray-500 mt-2">Chào mừng bạn quay trở lại!</p>
        </div>

        <Form
          name="login"
          onFinish={handleSubmit}
          layout="vertical"
          size="large"
        >
          <Form.Item
            name="username"
            label="Tên đăng nhập"
            rules={[
              { required: true, message: "Vui lòng nhập tên đăng nhập!" },
              { min: 5, message: "Tên đăng nhập phải có ít nhất 5 ký tự!" },
            ]}
          >
            <Input
              prefix={<UserOutlined className="text-gray-400" />}
              placeholder="Nhập tên đăng nhập"
            />
          </Form.Item>

          <Form.Item
            name="password"
            label="Mật khẩu"
            rules={[
              { required: true, message: "Vui lòng nhập mật khẩu!" },
              { min: 8, message: "Mật khẩu phải có ít nhất 8 ký tự!" },
            ]}
          >
            <Input.Password
              prefix={<LockOutlined className="text-gray-400" />}
              placeholder="Nhập mật khẩu"
            />
          </Form.Item>

          <div className="flex justify-between items-center mb-4">
            <button
              type="button"
              onClick={() => navigate("/forgot-password")}
              className="text-indigo-600 text-sm hover:underline"
            >
              Quên mật khẩu?
            </button>
          </div>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              className="w-full h-12 bg-indigo-600 hover:bg-indigo-700"
            >
              Đăng nhập
            </Button>
          </Form.Item>
        </Form>

        <div className="text-center mt-4">
          <span className="text-gray-600">Chưa có tài khoản? </span>
          <button
            type="button"
            onClick={() => navigate("/register-step1")}
            className="text-indigo-600 font-medium hover:underline"
          >
            Đăng ký ngay
          </button>
        </div>
      </Card>
    </div>
  );
};

export default LoginForm;
