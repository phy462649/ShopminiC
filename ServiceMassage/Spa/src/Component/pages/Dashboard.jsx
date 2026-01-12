import { Card, Row, Col, Statistic, Table, Tag } from 'antd';
import {
  UserOutlined,
  ShoppingCartOutlined,
  CalendarOutlined,
  DollarOutlined,
  RiseOutlined,
  FallOutlined,
} from '@ant-design/icons';

// Mock data - sẽ được thay thế bằng API calls
const stats = [
  {
    title: 'Tổng khách hàng',
    value: 1234,
    icon: <UserOutlined />,
    color: '#1890ff',
    change: 12,
    changeType: 'up',
  },
  {
    title: 'Đơn hàng hôm nay',
    value: 56,
    icon: <ShoppingCartOutlined />,
    color: '#52c41a',
    change: 8,
    changeType: 'up',
  },
  {
    title: 'Booking hôm nay',
    value: 23,
    icon: <CalendarOutlined />,
    color: '#722ed1',
    change: -3,
    changeType: 'down',
  },
  {
    title: 'Doanh thu tháng',
    value: 125000000,
    icon: <DollarOutlined />,
    color: '#fa8c16',
    prefix: '',
    suffix: ' đ',
    change: 15,
    changeType: 'up',
  },
];

const recentBookings = [
  { id: 1, customer: 'Nguyễn Văn A', service: 'Massage toàn thân', time: '09:00', status: 'confirmed' },
  { id: 2, customer: 'Trần Thị B', service: 'Chăm sóc da mặt', time: '10:30', status: 'pending' },
  { id: 3, customer: 'Lê Văn C', service: 'Massage chân', time: '14:00', status: 'completed' },
  { id: 4, customer: 'Phạm Thị D', service: 'Spa combo', time: '15:30', status: 'pending' },
];

const recentOrders = [
  { id: 1, customer: 'Nguyễn Văn E', total: 500000, items: 3, status: 'pending' },
  { id: 2, customer: 'Trần Văn F', total: 1200000, items: 5, status: 'shipped' },
  { id: 3, customer: 'Lê Thị G', total: 350000, items: 2, status: 'completed' },
];

const statusColors = {
  pending: 'orange',
  confirmed: 'blue',
  completed: 'green',
  cancelled: 'red',
  shipped: 'cyan',
};

const bookingColumns = [
  { title: 'Khách hàng', dataIndex: 'customer', key: 'customer' },
  { title: 'Dịch vụ', dataIndex: 'service', key: 'service' },
  { title: 'Giờ', dataIndex: 'time', key: 'time' },
  {
    title: 'Trạng thái',
    dataIndex: 'status',
    key: 'status',
    render: (status) => (
      <Tag color={statusColors[status]}>
        {status === 'pending' ? 'Chờ xử lý' : status === 'confirmed' ? 'Đã xác nhận' : 'Hoàn thành'}
      </Tag>
    ),
  },
];

const orderColumns = [
  { title: 'Khách hàng', dataIndex: 'customer', key: 'customer' },
  {
    title: 'Tổng tiền',
    dataIndex: 'total',
    key: 'total',
    render: (value) => value.toLocaleString('vi-VN') + ' đ',
  },
  { title: 'Số SP', dataIndex: 'items', key: 'items' },
  {
    title: 'Trạng thái',
    dataIndex: 'status',
    key: 'status',
    render: (status) => (
      <Tag color={statusColors[status]}>
        {status === 'pending' ? 'Chờ xử lý' : status === 'shipped' ? 'Đang giao' : 'Hoàn thành'}
      </Tag>
    ),
  },
];

export default function Dashboard() {
  return (
    <div className="space-y-6">
      {/* <h1 className="text-2xl font-bold text-gray-800">Dashboard</h1> */}

      {/* Stats Cards */}
      <Row gutter={[16, 16]}>
        {stats.map((stat, index) => (
          <Col xs={24} sm={12} lg={6} key={index}>
            <Card hoverable className="h-full">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-gray-500 text-sm mb-1">{stat.title}</p>
                  <Statistic
                    value={stat.value}
                    prefix={stat.prefix}
                    suffix={stat.suffix}
                    valueStyle={{ fontSize: '24px', fontWeight: 'bold' }}
                  />
                  <div className="mt-2 flex items-center text-sm">
                    {stat.changeType === 'up' ? (
                      <RiseOutlined className="text-green-500 mr-1" />
                    ) : (
                      <FallOutlined className="text-red-500 mr-1" />
                    )}
                    <span className={stat.changeType === 'up' ? 'text-green-500' : 'text-red-500'}>
                      {Math.abs(stat.change)}%
                    </span>
                    <span className="text-gray-400 ml-1">so với tháng trước</span>
                  </div>
                </div>
                <div
                  className="w-12 h-12 rounded-full flex items-center justify-center text-white text-xl"
                  style={{ backgroundColor: stat.color }}
                >
                  {stat.icon}
                </div>
              </div>
            </Card>
          </Col>
        ))}
      </Row>

      {/* Tables */}
      <Row gutter={[16, 16]}>
        <Col xs={24} lg={12}>
          <Card title="Booking hôm nay" extra={<a href="/admin/Booking">Xem tất cả</a>}>
            <Table
              dataSource={recentBookings}
              columns={bookingColumns}
              rowKey="id"
              pagination={false}
              size="small"
            />
          </Card>
        </Col>
        <Col xs={24} lg={12}>
          <Card title="Đơn hàng gần đây" extra={<a href="/admin/Orders">Xem tất cả</a>}>
            <Table
              dataSource={recentOrders}
              columns={orderColumns}
              rowKey="id"
              pagination={false}
              size="small"
            />
          </Card>
        </Col>
      </Row>
    </div>
  );
}
