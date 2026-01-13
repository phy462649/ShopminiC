# Testing Strategy - Bottom-Up Approach

## Cấu trúc Test

```
LandingPageApp.Tests/
├── Unit/                          # Unit Tests (Mock dependencies)
│   ├── Services/                  # Test Application Services
│   │   ├── CategoryServiceTests.cs
│   │   ├── ProductServiceTests.cs
│   │   └── ...
│   └── Validators/                # Test FluentValidation
│       ├── ProductValidatorTests.cs
│       └── ...
│
├── Integration/                   # Integration Tests (Real dependencies)
│   ├── Repositories/              # Test với InMemory DB
│   │   ├── CategoryRepositoryTests.cs
│   │   ├── ProductRepositoryTests.cs
│   │   └── ...
│   └── Api/                       # Test full API pipeline
│       ├── CategoryApiTests.cs
│       └── ...
│
└── README.md
```

## Thứ tự Test (Bottom-Up)

### Level 1: Repository Tests (Integration)
Test kết nối và query database.

```
Database ◄──── Repository
```

**Mục đích:**
- Verify CRUD operations
- Test query với filter, include, orderby
- Test relationship (FK)

**Cách chạy:**
```bash
dotnet test --filter "FullyQualifiedName~Integration.Repositories"
```

### Level 2: Service Tests (Unit)
Test business logic với mock repository.

```
Repository (Mock) ◄──── Service
```

**Mục đích:**
- Test business rules
- Test validation logic
- Test exception handling
- Test mapping Entity → DTO

**Cách chạy:**
```bash
dotnet test --filter "FullyQualifiedName~Unit.Services"
```

### Level 3: Validator Tests (Unit)
Test FluentValidation rules.

```
DTO ◄──── Validator
```

**Mục đích:**
- Test required fields
- Test format validation (email, phone)
- Test range validation

**Cách chạy:**
```bash
dotnet test --filter "FullyQualifiedName~Unit.Validators"
```

### Level 4: API Tests (Integration)
Test full HTTP pipeline.

```
HTTP Request ◄──── Controller ◄──── Service ◄──── Repository ◄──── Database
```

**Mục đích:**
- Test HTTP status codes
- Test request/response format
- Test authentication/authorization
- Test full flow

**Cách chạy:**
```bash
dotnet test --filter "FullyQualifiedName~Integration.Api"
```

## Chạy tất cả tests

```bash
# Chạy tất cả
dotnet test

# Chạy với coverage
dotnet test --collect:"XPlat Code Coverage"

# Chạy với verbose output
dotnet test --logger "console;verbosity=detailed"
```

## Test Pyramid

```
        /\
       /  \     API Tests (ít nhất)
      /----\
     /      \   Service Tests (nhiều)
    /--------\
   /          \ Repository Tests (nhiều)
  /------------\
```

## Naming Convention

- Test class: `{ClassName}Tests`
- Test method: `{MethodName}_{Scenario}_{ExpectedResult}`

Ví dụ:
- `GetByIdAsync_WhenExists_ShouldReturnCategory`
- `CreateAsync_WithDuplicateName_ShouldThrowBusinessException`

## AAA Pattern

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange - Setup test data và mocks
    var input = new CreateDto { Name = "Test" };
    
    // Act - Thực hiện action cần test
    var result = await _sut.CreateAsync(input);
    
    // Assert - Verify kết quả
    result.Should().NotBeNull();
    result.Name.Should().Be("Test");
}
```
