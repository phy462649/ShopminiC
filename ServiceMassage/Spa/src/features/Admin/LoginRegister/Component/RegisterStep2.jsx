import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Form, Input, Button, Card, Steps, message } from "antd";
import { UserOutlined, PhoneOutlined, HomeOutlined } from "@ant-design/icons";
import { authService } from "../../../../services";

const RegisterStep2 = () => {
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [step1Data, setStep1Data] = useState(null);

  useEffect(() => {
    // Lấy data từ step 1
    const savedData = sessionStorage.getItem("registerStep1");
    if (!savedData) {
      message.warning("Vui lòng hoàn thành bước 1 trước!");
      navigate("/register");
      return;
    }
    setStep1Data(JSON.parse(savedData));

    // Load lại dữ liệu step 2 nếu đã nhập trước đó (khi quay lại từ step 1)
    const savedStep2Data = sessionStorage.getItem("registerStep2");
    if (savedStep2Data) {
      const parsedStep2Data = JSON.parse(savedStep2Data);
      form.setFieldsValue({
        name: parsedStep2Data.name,
        phone: parsedStep2Data.phone,
        address: parsedStep2Data.address,
      });
    }
  }, [navigate, form]);

  const handleSubmit = async (values) => {
    if (!step1Data) {
      message.error("Không tìm thấy thông tin đăng ký!");
      navigate("/register");
      return;
    }

    setLoading(true);
    try {
      // Gộp data từ step 1 và step 2
      const registerData = {
        username: step1Data.username,
        email: step1Data.email,
        password: step1Data.password,
        confirmPassword: step1Data.confirmPassword,
        name: values.name,
        phone: values.phone,
        address: values.address,
      };

      const response = await authService.register(registerData);
      
      if (response.status) {
        message.success(response.message || "Đăng ký thành công! Vui lòng xác nhận email.");
        // Xóa data tạm
        sessionStorage.removeItem("registerStep1");
        sessionStorage.removeItem("registerStep2");
        // Chuyển đến trang xác nhận email
        navigate("/verify-user", { state: { email: step1Data.email } });
      } else {
        message.error(response.message || "Đăng ký thất bại!");
      }
    } catch (error) {
      console.error("Register error:", error);
      const errorMsg = error.response?.data?.message || error.message || "Đăng ký thất bại!";
      message.error(errorMsg);
    } finally {
      setLoading(false);
    }
  };

  const handleBack = () => {
    // Lưu dữ liệu step 2 vào sessionStorage trước khi quay lại
    const step2Values = form.getFieldsValue();
    sessionStorage.setItem("registerStep2", JSON.stringify(step2Values));
    navigate("/register");
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-100 to-purple-100 p-4">
      <Card className="w-full max-w-md shadow-xl">
        <Steps
          current={1}
          items={[
            { title: "Tài khoản" },
            { title: "Thông tin" },
          ]}
          className="mb-8"
        />

        <div className="text-center mb-6">
          <h1 className="text-2xl font-bold text-gray-800">Tạo tài khoản</h1>
          <p className="text-gray-500 mt-1">Bước 2: Thông tin cá nhân</p>
        </div>

        <Form
          form={form}
          name="register-step2"
          onFinish={handleSubmit}
          layout="vertical"
          size="large"
        >
          <Form.Item
            name="name"
            label="Họ và tên"
            rules={[
              { required: true, message: "Vui lòng nhập họ và tên!" },
              { max: 100, message: "Họ tên không quá 100 ký tự!" },
            ]}
          >
            <Input
              prefix={<UserOutlined className="text-gray-400" />}
              placeholder="Nhập họ và tên"
            />
          </Form.Item>

          <Form.Item
            name="phone"
            label="Số điện thoại"
            rules={[
              { required: true, message: "Vui lòng nhập số điện thoại!" },
              {
                pattern: /^\d{9,12}$/,
                message: "Số điện thoại phải từ 9-12 chữ số!",
              },
            ]}
          >
            <Input
              prefix={<PhoneOutlined className="text-gray-400" />}
              placeholder="Nhập số điện thoại"
              maxLength={12}
            />
          </Form.Item>

          <Form.Item
            name="address"
            label="Địa chỉ"
            rules={[
              { required: true, message: "Vui lòng nhập địa chỉ!" },
              { max: 255, message: "Địa chỉ không quá 255 ký tự!" },
            ]}
          >
            <Input.TextArea
              prefix={<HomeOutlined className="text-gray-400" />}
              placeholder="Nhập địa chỉ"
              rows={3}
              showCount
              maxLength={255}
            />
          </Form.Item>

          <Form.Item>
            <div className="flex gap-4">
              <Button
                onClick={handleBack}
                className="flex-1 h-12"
              >
                Quay lại
              </Button>
              <Button
                type="primary"
                htmlType="submit"
                loading={loading}
                className="flex-1 h-12 bg-indigo-600 hover:bg-indigo-700"
              >
                Hoàn thành đăng ký
              </Button>
            </div>
          </Form.Item>
        </Form>

        <div className="text-center">
          <span className="text-gray-600">Đã có tài khoản? </span>
          <button
            type="button"
            onClick={() => navigate("/login")}
            className="text-indigo-600 font-medium hover:underline"
          >
            Đăng nhập
          </button>
        </div>
      </Card>
    </div>
  );
};

export default RegisterStep2;
