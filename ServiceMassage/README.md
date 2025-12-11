# ServiceMassage - Massage Service Management System

A comprehensive massage service booking and management system built with Clean Architecture, ASP.NET Core Web API, and React SPA.

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

- .NET 9.0 SDK
- Node.js 18+ and npm
- SQL Server
- Redis (optional, for caching)

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
   - API: `https://localhost:5001` (or `http://localhost:5000`)
   - Frontend: `http://localhost:5173`
   - Swagger: `https://localhost:5001/swagger`

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

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions, please open an issue on GitHub or contact the development team.

---

**Built with ❤️ using Clean Architecture principles**
