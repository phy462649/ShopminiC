import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Form, Input, Button, message, Tabs, Avatar, Upload, Spin } from "antd";
import { UserOutlined, MailOutlined, PhoneOutlined, HomeOutlined, CameraOutlined, LockOutlined, SettingOutlined } from "@ant-design/icons";
import { useMutation, useQuery } from "@tanstack/react-query";
import { authService } from "../../services";
import apiClient, { userApiClient } from "../../services/apiClient";

export default function Profile() {
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [passwordForm] = Form.useForm();
  const user = authService.getCurrentUser();
  const [activeTab, setActiveTab] = useState("info");

  // Redirect if not logged in
  if (!user && !authService.isAuthenticated()) {
    navigate("/login");
    return null;
  }

  // Check if user is admin via API
  const { data: isAdminData, isLoading: isAdminLoading } = useQuery({
    queryKey: ["isAdmin"],
    queryFn: () => apiClient.get("/Auth/isAdmin"),
    enabled: authService.isAuthenticated(),
    retry: false,
  });

  const isAdmin = isAdminData?.status === true || isAdminData === true;

  // Update profile mutation
  const updateProfileMutation = useMutation({
    mutationFn: (data) => userApiClient.put("/profile", data),
    onSuccess: (response) => {
      localStorage.setItem("user", JSON.stringify({ ...user, ...response }));
      message.success("Cập nhật thông tin thành công!");
    },
    onError: () => {
      message.error("Cập nhật thất bại!");
    },
  });

  // Change password mutation
  const changePasswordMutation = useMutation({
    mutationFn: (data) => apiClient.post("/Auth/change-password", data),
    onSuccess: () => {
      message.success("Đổi mật khẩu thành công!");
      passwordForm.resetFields();
    },
    onError: () => {
      message.error("Đổi mật khẩu thất bại!");
    },
  });

  const handleUpdateProfile = (values) => {
    updateProfileMutation.mutate(values);
  };

  const handleChangePassword = (values) => {
    changePasswordMutation.mutate({
      currentPassword: values.currentPassword,
      newPassword: values.newPassword,
    });
  };

  const tabItems = [
    {
      key: "info",
      label: "Thông tin cá nhân",
      children: (
        <div className="max-w-xl">
          <Form
            form={form}
            layout="vertical"
            initialValues={{
              fullName: user?.fullName || user?.name || "",
              email: user?.email || "",
              phone: user?.phone || "",
              address: user?.address || "",
            }}
            onFinish={handleUpdateProfile}
          >
            <Form.Item
              name="fullName"
              label="Họ và tên"
              rules={[{ required: true, message: "Vui lòng nhập họ tên!" }]}
            >
              <Input prefix={<UserOutlined />} placeholder="Họ và tên" size="large" />
            </Form.Item>

            <Form.Item
              name="email"
              label="Email"
            >
              <Input prefix={<MailOutlined />} placeholder="Email" size="large" disabled />
            </Form.Item>

            <Form.Item
              name="phone"
              label="Số điện thoại"
              rules={[{ pattern: /^[0-9]{10}$/, message: "Số điện thoại không hợp lệ!" }]}
            >
              <Input prefix={<PhoneOutlined />} placeholder="Số điện thoại" size="large" />
            </Form.Item>

            <Form.Item
              name="address"
              label="Địa chỉ"
            >
              <Input prefix={<HomeOutlined />} placeholder="Địa chỉ" size="large" />
            </Form.Item>

            <Form.Item>
              <Button
                type="primary"
                htmlType="submit"
                loading={updateProfileMutation.isPending}
                className="bg-pink-500 hover:bg-pink-600"
                size="large"
              >
                Cập nhật thông tin
              </Button>
            </Form.Item>
          </Form>
        </div>
      ),
    },
    {
      key: "password",
      label: "Đổi mật khẩu",
      children: (
        <div className="max-w-xl">
          <Form
            form={passwordForm}
            layout="vertical"
            onFinish={handleChangePassword}
          >
            <Form.Item
              name="currentPassword"
              label="Mật khẩu hiện tại"
              rules={[{ required: true, message: "Vui lòng nhập mật khẩu hiện tại!" }]}
            >
              <Input.Password prefix={<LockOutlined />} placeholder="Mật khẩu hiện tại" size="large" />
            </Form.Item>

            <Form.Item
              name="newPassword"
              label="Mật khẩu mới"
              rules={[
                { required: true, message: "Vui lòng nhập mật khẩu mới!" },
                { min: 6, message: "Mật khẩu phải có ít nhất 6 ký tự!" },
              ]}
            >
              <Input.Password prefix={<LockOutlined />} placeholder="Mật khẩu mới" size="large" />
            </Form.Item>

            <Form.Item
              name="confirmPassword"
              label="Xác nhận mật khẩu mới"
              dependencies={["newPassword"]}
              rules={[
                { required: true, message: "Vui lòng xác nhận mật khẩu!" },
                ({ getFieldValue }) => ({
                  validator(_, value) {
                    if (!value || getFieldValue("newPassword") === value) {
                      return Promise.resolve();
                    }
                    return Promise.reject(new Error("Mật khẩu xác nhận không khớp!"));
                  },
                }),
              ]}
            >
              <Input.Password prefix={<LockOutlined />} placeholder="Xác nhận mật khẩu mới" size="large" />
            </Form.Item>

            <Form.Item>
              <Button
                type="primary"
                htmlType="submit"
                loading={changePasswordMutation.isPending}
                className="bg-pink-500 hover:bg-pink-600"
                size="large"
              >
                Đổi mật khẩu
              </Button>
            </Form.Item>
          </Form>
        </div>
      ),
    },
  ];

  // Add admin tab if user is admin
    // if (isAdmin) {
    //   tabItems.push({
    //     key: "admin",
    //     label: "Quản trị",
    //     children: (
    //       <div className="max-w-xl">
    //         <div className="text-center py-8">
    //           <SettingOutlined className="text-6xl text-pink-500 mb-4" />
    //           <h3 className="text-xl font-semibold text-gray-800 mb-2">Trang quản trị</h3>
    //           <p className="text-gray-500 mb-6">
    //             Bạn có quyền truy cập vào trang quản trị hệ thống
    //           </p>
    //           <Button
    //             type="primary"
    //             size="large"
    //             icon={<SettingOutlined />}
    //             onClick={() => navigate("/admin")}
    //             className="bg-pink-500 hover:bg-pink-600"
    //           >
    //             Đi đến trang Admin
    //           </Button>
    //         </div>
    //       </div>
    //     ),
    //   });
    // }

  // if (isAdminLoading) {
  //   return (
  //     <div className="min-h-screen bg-gray-50 flex items-center justify-center">
  //       <Spin size="large" />
  //     </div>
  //   );
  // }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-4xl mx-auto px-6">
        {/* Header */}
        <div className="bg-white rounded-lg shadow-sm p-6 mb-6">
          <div className="flex items-center gap-6">
            <div className="relative">
              <Avatar
                size={100}
                src={user?.avatar}
                icon={<UserOutlined />}
                className="bg-pink-500"
              />
              <Upload
                showUploadList={false}
                beforeUpload={() => false}
                className="absolute bottom-0 right-0"
              >
                <Button
                  shape="circle"
                  icon={<CameraOutlined />}
                  size="small"
                  className="bg-white shadow"
                />
              </Upload>
            </div>
            <div>
              <h1 className="text-2xl font-bold text-gray-800">
                {user?.fullName || user?.name || "Người dùng"}
              </h1>
              <p className="text-gray-500">{user?.email}</p>
              <p className="text-sm text-pink-500 mt-1">
                {user?.roleName || user?.role || "Khách hàng"}
              </p>
            </div>
          </div>
        </div>

        {/* Content */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <Tabs
            activeKey={activeTab}
            onChange={setActiveTab}
            items={tabItems}
          />
        </div>
      </div>
    </div>
  );
}
