# Test API script
$baseUrl = "http://localhost:5092"

# Test Register
Write-Host "Testing Register API..." -ForegroundColor Yellow
$registerBody = @{
    username = "testuser"
    email = "test@example.com"
    password = "TestPassword123"
    confirmPassword = "TestPassword123"
    name = "Test User"
    phone = "0123456789"
    address = "Test Address"
} | ConvertTo-Json

try {
    $registerResponse = Invoke-WebRequest -Uri "$baseUrl/api/auth/register" -Method POST -ContentType "application/json" -Body $registerBody
    Write-Host "Register Status:" $registerResponse.StatusCode -ForegroundColor Green
    Write-Host "Register Response:" $registerResponse.Content -ForegroundColor Green
} catch {
    Write-Host "Register Error:" $_.Exception.Message -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $responseBody = $reader.ReadToEnd()
        Write-Host "Register Response:" $responseBody -ForegroundColor Red
    }
}

# Test Login
Write-Host "`nTesting Login API..." -ForegroundColor Yellow
$loginBody = @{
    username = "testuser"
    password = "TestPassword123"
    deviceToken = "256ce561-f260-4094-9920-d88a06f02243"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-WebRequest -Uri "$baseUrl/api/auth/login" -Method POST -ContentType "application/json" -Body $loginBody
    Write-Host "Login Status:" $loginResponse.StatusCode -ForegroundColor Green
    Write-Host "Login Response:" $loginResponse.Content -ForegroundColor Green
} catch {
    Write-Host "Login Error:" $_.Exception.Message -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $responseBody = $reader.ReadToEnd()
        Write-Host "Login Response:" $responseBody -ForegroundColor Red
    }
}





