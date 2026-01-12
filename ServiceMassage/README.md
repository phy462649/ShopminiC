# ServiceMassage - Massage Service Management System

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-blue.svg)](https://reactjs.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

> A comprehensive massage service booking and management system built with Clean Architecture, ASP.NET Core Web API, and React SPA.

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [API Documentation](#-api-documentation)
- [Database Schema](#-database-schema)
- [Development](#-development)
- [Testing](#-testing)
- [Deployment](#-deployment)
- [Contributing](#-contributing)
- [License](#-license)
- [Support](#-support)
- [Changelog](#-changelog)

## ✨ Features

### 🔐 Authentication & Authorization
- JWT-based authentication with refresh tokens
- Role-based authorization (Admin, Staff, Customer)
- Password reset and email verification
- Secure password hashing with BCrypt

### 📅 Booking Management
- Online booking system for massage services
- Real-time availability checking
- Booking confirmation and notifications
- Booking history and management

### 👥 User Management
- Multi-role user system
- Advanced user search and filtering
- Profile management
- User activity tracking

### 💳 Payment & Orders
- Secure payment processing
- Product catalog management
- Order tracking and management
- Invoice generation

### 📊 Staff & Scheduling
- Staff schedule management
- Availability tracking
- Service assignment
- Performance analytics

### 🏢 Room Management
- Room availability tracking
- Room assignment for services
- Maintenance scheduling

### 📱 Modern UI/UX
- Responsive React frontend
- Intuitive booking interface
- Admin dashboard
- Mobile-friendly design

## 🏗️ Architecture

This project follows **Clean Architecture** principles with the following layers:

- **Domain Layer**: Core business entities, interfaces, and business rules
- **Application Layer**: Application services, DTOs, and business logic
- **Infrastructure Layer**: Data access, external services, and implementations
- **API Layer**: RESTful API controllers and middleware
- **SPA**: React frontend application

## 🛠️ Tech Stack

### Backend
- **ASP.NET Core 9.0** - Web API framework
- **Entity Framework Core 9.0** - ORM for database operations
- **SQL Server** - Primary database
- **Redis** - Caching layer
- **JWT** - Authentication & Authorization
- **BCrypt** - Password hashing
- **Swagger/OpenAPI** - API documentation

### Frontend
- **React 18** - UI framework
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Utility-first CSS framework
- **ESLint** - Code linting

## 🚀 Getting Started

### Prerequisites

**Required:**
- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **SQL Server** (2019 or later) - [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

**Recommended:**
- **Redis** (for caching) - [Download](https://redis.io/download)
- **Visual Studio 2022** or **VS Code** - [Download](https://visualstudio.microsoft.com/) or [Download](https://code.visualstudio.com/)
- **SQL Server Management Studio** - [Download](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

**System Requirements:**
- Windows 10/11, macOS, or Linux
- 8GB RAM minimum, 16GB recommended
- 5GB free disk space

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ServiceMassage
   ```

2. **Setup Backend**
   ```bash
   # Restore packages
   dotnet restore

   # Update database (make sure SQL Server is running)
   cd LandingPageApp.Api
   dotnet ef database update

   # Run the API
   dotnet run
   ```

3. **Setup Frontend**
   ```bash
   cd Spa
   npm install
   npm run dev
   ```

4. **Access the application**
   - 🌐 **Frontend**: `http://localhost:5173`
   - 🔌 **API**: `https://localhost:5001` (or `http://localhost:5000`)
   - 📚 **Swagger Documentation**: `https://localhost:5001/swagger`
   - 🗄️ **Database**: Check connection string in `appsettings.json`

### Quick Verification

After starting both backend and frontend, you should see:
- ✅ API health check: `GET https://localhost:5001/health`
- ✅ Swagger UI loads at `/swagger`
- ✅ Frontend displays login page
- ✅ Database migrations applied

## 📁 Project Structure

```
ServiceMassage/
├── LandingPageApp.Api/           # API Layer
│   ├── Controllers/              # API Controllers
│   ├── Middlewares/              # Custom middlewares
│   ├── Extensions/               # Service extensions
│   ├── appsettings.json          # App configuration
│   └── Program.cs                # Application entry point
├── LandingPageApp.Application/   # Application Layer
│   ├── Services/                 # Application services
│   ├── Dtos/                     # Data Transfer Objects
│   ├── Interfaces/               # Service interfaces
│   └── Validations/              # Input validations
├── LandingPageApp.Domain/        # Domain Layer
│   ├── Entities/                 # Domain entities
│   ├── Repositories/             # Repository interfaces
│   ├── Enums/                    # Domain enums
│   └── Events/                   # Domain events
├── LandingPageApp.Infrastructure/# Infrastructure Layer
│   ├── Data/                     # Database context
│   ├── Repositories/             # Repository implementations
│   └── Caching/                  # Caching implementations
├── Spa/                          # React Frontend
│   ├── src/                      # Source code
│   ├── public/                   # Static assets
│   └── package.json              # Dependencies
└── LandingPageApp.sln            # Solution file
```

## 📸 Screenshots

### Frontend Interface
*Coming soon - screenshots of the React SPA interface*

### Admin Dashboard
*Coming soon - admin panel screenshots*

### API Documentation
![Swagger UI](https://via.placeholder.com/800x400/4CAF50/white?text=Swagger+API+Documentation)

## 🔑 Authentication & Authorization

The system uses JWT tokens for authentication with role-based authorization:

- **Admin**: Full system access
- **Staff**: Limited access for service operations
- **Customer**: Access to booking and personal data

## 📚 API Documentation

### Authentication Endpoints

```http
POST /api/auth/login
POST /api/auth/register
POST /api/auth/refresh
POST /api/auth/logout
POST /api/auth/request-password-reset
POST /api/auth/reset-password
POST /api/auth/verify-email
```

### Person Management

```http
GET    /api/person              # Get all persons
GET    /api/person/{id}         # Get person by ID
GET    /api/person/search       # Advanced search with filters
POST   /api/person              # Create new person
PUT    /api/person/{id}         # Update person
DELETE /api/person/{id}         # Delete person
```

#### Person Search API

The search endpoint supports advanced filtering and pagination:

**Query Parameters:**
- `search`: Global search in Name, Email, Phone, Username
- `name`: Filter by name
- `email`: Filter by email
- `phone`: Filter by phone
- `username`: Filter by username
- `roleId`: Filter by role ID
- `createdFrom`: Filter by creation date from (YYYY-MM-DD)
- `createdTo`: Filter by creation date to (YYYY-MM-DD)
- `sortBy`: Sort field (Id, Name, Email, Phone, Username, RoleId, CreatedAt, UpdatedAt)
- `sortOrder`: Sort order (asc, desc)
- `page`: Page number (1-based)
- `pageSize`: Items per page (1-100)

**Example:**
```http
GET /api/person/search?search=john&email=gmail.com&sortBy=CreatedAt&sortOrder=desc&page=1&pageSize=10
```

### Other Entity Endpoints

The system provides full CRUD operations for:
- **Bookings** (`/api/booking`)
- **Categories** (`/api/category`)
- **Orders** (`/api/order`)
- **Order Items** (`/api/order-item`)
- **Payments** (`/api/payment`)
- **Products** (`/api/product`)
- **Roles** (`/api/role`)
- **Rooms** (`/api/room`)
- **Services** (`/api/services`)
- **Staff Schedules** (`/api/staff-schedule`)

## 🗄️ Database Schema

### Main Entities

- **Person**: Users of the system (Customers, Staff, Admins)
- **Role**: User roles and permissions
- **Booking**: Service bookings
- **Service**: Available massage services
- **Product**: Sellable products
- **Order**: Product orders
- **OrderItem**: Order line items
- **Payment**: Payment transactions
- **Room**: Service rooms
- **StaffSchedule**: Staff working schedules
- **Category**: Product/service categories

## 🔧 Development

### Code Style & Standards

- Follow C# coding conventions
- Use async/await for I/O operations
- Implement proper error handling
- Write unit tests for business logic
- Use meaningful commit messages

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Building for Production

```bash
# Build backend
dotnet publish LandingPageApp.Api -c Release -o ./publish

# Build frontend
cd Spa
npm run build
```

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test LandingPageApp.Tests

# Run tests in watch mode
dotnet watch test
```

### Test Coverage

The project maintains high test coverage with unit tests for:
- Application services
- Domain logic
- API controllers
- Validation rules

## 🚀 Deployment

### Prerequisites for Production

- Windows/Linux Server with .NET 9.0 runtime
- SQL Server database
- Redis server (recommended)
- Reverse proxy (nginx/IIS)

### Production Deployment

1. **Configure Environment**
   ```bash
   # Set production environment variables
   export ASPNETCORE_ENVIRONMENT=Production
   export ConnectionStrings__DefaultConnection="Server=...;Database=...;User Id=...;Password=..."
   export ConnectionStrings__Redis="localhost:6379"
   ```

2. **Deploy Backend**
   ```bash
   # Publish application
   dotnet publish LandingPageApp.Api -c Release -o ./publish

   # Run migrations
   dotnet ef database update --project LandingPageApp.Api

   # Start application
   dotnet LandingPageApp.Api.dll --urls "http://0.0.0.0:5000"
   ```

3. **Deploy Frontend**
   ```bash
   cd Spa
   npm run build
   # Copy dist folder to web server
   ```

### Docker Deployment (Optional)

```dockerfile
# Build Docker image
docker build -t massageservice .

# Run with docker-compose
docker-compose up -d
```

## 🤝 Contributing

We welcome contributions! Please follow these steps:

### Development Workflow

1. **Fork the repository**
   ```bash
   git clone https://github.com/yourusername/servicemassage.git
   cd servicemassage
   ```

2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or for bug fixes
   git checkout -b fix/issue-description
   ```

3. **Setup development environment**
   ```bash
   # Backend setup
   dotnet restore
   dotnet ef database update

   # Frontend setup
   cd Spa
   npm install
   ```

4. **Make your changes**
   - Follow the coding standards
   - Write tests for new features
   - Update documentation as needed

5. **Run tests and linting**
   ```bash
   # Run backend tests
   dotnet test

   # Run frontend linting
   cd Spa
   npm run lint
   ```

6. **Commit your changes**
   ```bash
   git add .
   git commit -m "feat: add amazing new feature"
   ```

7. **Push and create PR**
   ```bash
   git push origin feature/your-feature-name
   ```
   Then open a Pull Request on GitHub

### Commit Message Convention

We follow [Conventional Commits](https://conventionalcommits.org/):
- `feat:` - New features
- `fix:` - Bug fixes
- `docs:` - Documentation changes
- `style:` - Code style changes
- `refactor:` - Code refactoring
- `test:` - Testing
- `chore:` - Maintenance

### Code Standards

- **C#**: Follow Microsoft's coding conventions
- **JavaScript/React**: Use ESLint rules
- **API**: RESTful design principles
- **Security**: Input validation and sanitization

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions:
- 📧 **Email**: support@servicemassage.com
- 🐛 **Issues**: [GitHub Issues](https://github.com/your-repo/issues)
- 📖 **Documentation**: [Wiki](https://github.com/your-repo/wiki)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/your-repo/discussions)

### Common Issues

**Database Connection Issues:**
```bash
# Check SQL Server connection
sqlcmd -S localhost -U sa -P yourpassword -Q "SELECT @@VERSION"
```

**Redis Connection Issues:**
```bash
# Test Redis connection
redis-cli ping
```

## 📝 Changelog

### [v1.0.0] - 2024-12-XX
- ✨ Initial release
- 🔐 JWT authentication with role-based authorization
- 📅 Complete booking management system
- 👥 Multi-role user management
- 💳 Payment and order processing
- 📊 Staff scheduling system
- 🏢 Room management
- 📱 Responsive React frontend
- 🧪 Comprehensive test coverage
- 📚 Swagger API documentation

### [v0.9.0] - 2024-11-XX (Beta)
- 🏗️ Clean Architecture implementation
- 🔑 Authentication system
- 📋 Basic CRUD operations
- 🎨 Frontend UI components

## 👥 Authors & Contributors

### Core Team
- **Project Lead**: [Your Name](https://github.com/yourusername)
- **Backend Developer**: [Developer Name](https://github.com/developer)
- **Frontend Developer**: [Developer Name](https://github.com/developer)

### Contributors
<a href="https://github.com/your-repo/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=your-repo/servicemassage" />
</a>

## 🙏 Acknowledgments

- Thanks to the .NET community for excellent documentation
- React community for amazing UI components
- All contributors who helped shape this project

---

**Built with ❤️ using Clean Architecture principles**
