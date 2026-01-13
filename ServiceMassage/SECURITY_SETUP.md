# Hướng dẫn cấu hình bảo mật

## 1. Sử dụng User Secrets (Development)

User Secrets lưu trữ credentials an toàn trên máy local, KHÔNG commit lên Git.

### Khởi tạo User Secrets
```bash
cd LandingPageApp.Api
dotnet user-secrets init
```

### Thêm secrets
```bash
# Database
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;User=root;Password=YOUR_PASSWORD;Database=massageservice"

# JWT
dotnet user-secrets set "Jwt:Secret" "your-super-secret-jwt-key-at-least-32-characters-long"

# SMTP Email
dotnet user-secrets set "Smtp:User" "your-email@gmail.com"
dotnet user-secrets set "Smtp:Password" "your-app-password"
dotnet user-secrets set "Smtp:FromEmail" "your-email@gmail.com"

# Google OAuth
dotnet user-secrets set "Google:ClientId" "your-google-client-id"
dotnet user-secrets set "Google:ClientSecret" "your-google-client-secret"

# Cloudinary
dotnet user-secrets set "Cloudinary:CloudName" "your-cloud-name"
dotnet user-secrets set "Cloudinary:ApiKey" "your-api-key"
dotnet user-secrets set "Cloudinary:ApiSecret" "your-api-secret"

# VnPay
dotnet user-secrets set "VnPay:TmnCode" "your-tmn-code"
dotnet user-secrets set "VnPay:HashSecret" "your-hash-secret"
dotnet user-secrets set "VnPay:ReturnUrl" "https://your-domain/api/vnpay/return"
```

### Xem tất cả secrets
```bash
dotnet user-secrets list
```

### Xóa secret
```bash
dotnet user-secrets remove "Key:Name"
```

## 2. Production - Environment Variables

Trong production, sử dụng environment variables hoặc secret management service.

### Docker Compose
Tạo file `.env` (đã được gitignore):
```env
MYSQL_PASSWORD=your_secure_password
JWT_SECRET=your-production-jwt-secret-at-least-32-chars
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
CLOUDINARY_CLOUD_NAME=your-cloud
CLOUDINARY_API_KEY=your-key
CLOUDINARY_API_SECRET=your-secret
```

### Azure/AWS
- Azure: Key Vault
- AWS: Secrets Manager / Parameter Store

## 3. Checklist bảo mật

- [ ] KHÔNG commit credentials vào Git
- [ ] Sử dụng User Secrets cho development
- [ ] Sử dụng Environment Variables cho production
- [ ] Kiểm tra .gitignore đã ignore các file nhạy cảm
- [ ] Rotate secrets định kỳ
- [ ] Sử dụng App Password cho Gmail (không dùng password chính)
- [ ] JWT Secret ít nhất 32 ký tự

## 4. Kiểm tra secrets bị lộ

```bash
# Tìm hardcoded passwords
git grep -i "password"
git grep -i "secret"
git grep -i "apikey"

# Kiểm tra git history
git log -p --all -S "password" -- "*.json" "*.cs"
```

## 5. Nếu đã lộ secrets

1. **Ngay lập tức** đổi tất cả credentials bị lộ
2. Revoke API keys cũ
3. Kiểm tra logs xem có truy cập bất thường không
4. Sử dụng `git filter-branch` hoặc BFG Repo-Cleaner để xóa khỏi history
