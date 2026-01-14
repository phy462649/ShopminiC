using LandingPageApp.Api.Extensions;
using LandingPageApp.Api.Filters;
using LandingPageApp.Api.Middlewares;
using LandingPageApp.Application.EventHandlers;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Application.Services;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Events;
using LandingPageApp.Infrastructure.Caching;
using LandingPageApp.Infrastructure.Data;
using LandingPageApp.Infrastructure.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===========================================
// DATABASE & CACHING
// ===========================================
builder.Services.AddDbContext<ServicemassageContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Redis connection for direct access (to list all keys)
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(redisConnectionString));
}

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "LandingPage_";
});

// ===========================================
// AUTOMAPPER & VALIDATORS
// ===========================================
builder.Services.AddMappers();
builder.Services.AddValidators();

// ===========================================
// HEALTH CHECKS
// ===========================================
builder.Services.AddHealthChecksConfiguration(builder.Configuration);

// ===========================================
// APPLICATION SERVICES
// ===========================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<ICacheRediservice, RedisCacheService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookingService, LandingPageApp.Application.Services.BookingService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IStaffScheduleService, StaffScheduleService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IServicesService, ServicesService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IBookingServiceService, BookingServiceService>();

// ===========================================
// DOMAIN EVENTS
// ===========================================
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<IEventHandler<BookingCreatedEvent>, BookingCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<BookingConfirmedEvent>, BookingConfirmedEventHandler>();
builder.Services.AddScoped<IEventHandler<BookingCancelledEvent>, BookingCancelledEventHandler>();
builder.Services.AddScoped<IEventHandler<BookingCompletedEvent>, BookingCompletedEventHandler>();

// ===========================================
// REPOSITORIES
// ===========================================
builder.Services.AddRepositories();

// ===========================================
// EMAIL SERVICE
// ===========================================
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, EmailService>();

// ===========================================
// CLOUDINARY SERVICE
// ===========================================
builder.Services.Configure<LandingPageApp.Infrastructure.Services.CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<ICloudinaryService, LandingPageApp.Infrastructure.Services.CloudinaryService>();

// ===========================================
// VNPAY SERVICE
// ===========================================
builder.Services.Configure<LandingPageApp.Infrastructure.Services.VnPaySettings>(
    builder.Configuration.GetSection("VnPay"));
builder.Services.AddScoped<IVnPayService, LandingPageApp.Infrastructure.Services.VnPayService>();

// ===========================================
// JWT AUTHENTICATION
// ===========================================
var jwtSecret = builder.Configuration["Jwt:Secret"] 
    ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ===========================================
// CONTROLLERS & FILTERS
// ===========================================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// ===========================================
// SWAGGER / OPENAPI
// ===========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ServiceMassage API",
        Version = "v1",
        Description = "API for Massage Service Booking System",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@servicemassage.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token (KHÔNG cần gõ Bearer)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===========================================
// RESPONSE CACHING & COMPRESSION
// ===========================================
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression();

// ===========================================
// CORS
// ===========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ===========================================
// MIDDLEWARE PIPELINE
// ===========================================
app.UseRequestTiming(); // Đo thời gian response
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable CORS
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiceMassage API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseResponseCompression();

app.UseAuthentication();
app.UseAuthorization();

// ===========================================
// ENDPOINTS
// ===========================================
app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();

// Expose Program class for integration tests
public partial class Program { }
