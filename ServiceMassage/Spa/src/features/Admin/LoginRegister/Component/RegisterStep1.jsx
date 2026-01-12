import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Form, Input, Button, Card, Steps } from "antd";
import { UserOutlined, MailOutlined, LockOutlined } from "@ant-design/icons";

const RegisterStep1 = () => {
  const navigate = useNavigate();
  const [form] = Form.useForm();

  // Load dữ liệu đã lưu từ sessionStorage khi quay lại từ step 2
  useEffect(() => {
    const savedData = sessionStorage.getItem("registerStep1");
    if (savedData) {
      const parsedData = JSON.parse(savedData);
      form.setFieldsValue({
        username: parsedData.username,
        email: parsedData.email,
        password: parsedData.password,
        confirmPassword: parsedData.confirmPassword,
      });
    }
  }, [form]);

  const handleSubmit = (values) => {
    // Lưu data vào sessionStorage để dùng ở step 2
    sessionStorage.setItem("registerStep1", JSON.stringify(values));
    navigate("/register-step2");
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-100 to-purple-100 p-4">
      <Card className="w-full max-w-md shadow-xl">
        <Steps
          current={0}
          items={[
            { title: "Tài khoản" },
            { title: "Thông tin" },
          ]}
          className="mb-8"
        />

        <div className="text-center mb-6">
          <h1 className="text-2xl font-bold text-gray-800">Tạo tài khoản</h1>
          <p className="text-gray-500 mt-1">Bước 1: Thông tin đăng nhập</p>
        </div>

        <Form
          form={form}
          name="register-step1"
          onFinish={handleSubmit}
          layout="vertical"
          size="large"
        >
          <Form.Item
            name="username"
            label="Tên đăng nhập"
            rules={[
              { required: true, message: "Vui lòng nhập tên đăng nhập!" },
              { min: 4, message: "Tên đăng nhập phải có ít nhất 4 ký tự!" },
              { max: 30, message: "Tên đăng nhập không quá 30 ký tự!" },
            ]}
          >
            <Input
              prefix={<UserOutlined className="text-gray-400" />}
              placeholder="Nhập tên đăng nhập"
            />
          </Form.Item>

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
              placeholder="Nhập email"
            />
          </Form.Item>

          <Form.Item
            name="password"
            label="Mật khẩu"
            rules={[
              { required: true, message: "Vui lòng nhập mật khẩu!" },
              { min: 6, message: "Mật khẩu phải có ít nhất 6 ký tự!" },
              {
                pattern: /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$/,
                message: "Mật khẩu phải có chữ hoa, chữ thường và số!",
              },
            ]}
          >
            <Input.Password
              prefix={<LockOutlined className="text-gray-400" />}
              placeholder="Nhập mật khẩu"
            />
          </Form.Item>

          <Form.Item
            name="confirmPassword"
            label="Xác nhận mật khẩu"
            dependencies={["password"]}
            rules={[
              { required: true, message: "Vui lòng xác nhận mật khẩu!" },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue("password") === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error("Mật khẩu không khớp!"));
                },
              }),
            ]}
          >
            <Input.Password
              prefix={<LockOutlined className="text-gray-400" />}
              placeholder="Nhập lại mật khẩu"
            />
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              className="w-full h-12 bg-indigo-600 hover:bg-indigo-700"
            >
              Tiếp tục
            </Button>
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

export default RegisterStep1;
