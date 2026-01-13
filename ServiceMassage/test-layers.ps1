# ============================================
# TEST TỪNG LAYER - Bottom Up Testing
# ============================================

param(
    [Parameter(Position=0)]
    [ValidateSet("all", "repo", "service", "validator", "controller", "api")]
    [string]$Layer = "all"
)

$ErrorActionPreference = "Continue"

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  BOTTOM-UP TESTING BY LAYER" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

function Test-Layer {
    param(
        [string]$Name,
        [string]$Filter,
        [string]$Description
    )
    
    Write-Host "`n----------------------------------------" -ForegroundColor Yellow
    Write-Host "Layer: $Name" -ForegroundColor Yellow
    Write-Host "Description: $Description" -ForegroundColor Gray
    Write-Host "----------------------------------------" -ForegroundColor Yellow
    
    dotnet test LandingPageApp.Tests --filter $Filter --no-build --verbosity normal
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n✓ $Name tests PASSED" -ForegroundColor Green
    } else {
        Write-Host "`n✗ $Name tests FAILED" -ForegroundColor Red
    }
}

# Build first
Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build LandingPageApp.sln --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

switch ($Layer) {
    "repo" {
        Test-Layer -Name "REPOSITORY" `
                   -Filter "FullyQualifiedName~Integration.Repositories" `
                   -Description "Test kết nối DB, CRUD operations"
    }
    "service" {
        Test-Layer -Name "SERVICE" `
                   -Filter "FullyQualifiedName~Unit.Services" `
                   -Description "Test business logic với mock repository"
    }
    "validator" {
        Test-Layer -Name "VALIDATOR" `
                   -Filter "FullyQualifiedName~Unit.Validators" `
                   -Description "Test FluentValidation rules"
    }
    "controller" {
        Test-Layer -Name "CONTROLLER" `
                   -Filter "FullyQualifiedName~Unit.Controllers" `
                   -Description "Test controller với mock service"
    }
    "api" {
        Test-Layer -Name "API" `
                   -Filter "FullyQualifiedName~Integration.Api" `
                   -Description "Test full HTTP pipeline"
    }
    "all" {
        Write-Host "`n🔄 Testing ALL layers from bottom to top...`n" -ForegroundColor Magenta
        
        # Layer 1: Repository
        Test-Layer -Name "1. REPOSITORY (Bottom)" `
                   -Filter "FullyQualifiedName~Integration.Repositories" `
                   -Description "Test kết nối DB, CRUD operations"
        
        # Layer 2: Service
        Test-Layer -Name "2. SERVICE" `
                   -Filter "FullyQualifiedName~Unit.Services" `
                   -Description "Test business logic với mock repository"
        
        # Layer 3: Validator
        Test-Layer -Name "3. VALIDATOR" `
                   -Filter "FullyQualifiedName~Unit.Validators" `
                   -Description "Test FluentValidation rules"
        
        # Layer 4: Controller
        Test-Layer -Name "4. CONTROLLER" `
                   -Filter "FullyQualifiedName~Unit.Controllers" `
                   -Description "Test controller với mock service"
        
        # Layer 5: API
        Test-Layer -Name "5. API (Top)" `
                   -Filter "FullyQualifiedName~Integration.Api" `
                   -Description "Test full HTTP pipeline"
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  TESTING COMPLETED" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan
