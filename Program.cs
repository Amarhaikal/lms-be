using QUANTM;
using QUANTM.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                         ?? new[] { "http://localhost:3000" };

    options.AddPolicy("BankingPolicy",
        builder =>
        {
            builder.WithOrigins(allowedOrigins)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // Allow cookies/auth headers
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // Add JWT Bearer security scheme
        document.Components ??= new Microsoft.OpenApi.Models.OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, Microsoft.OpenApi.Models.OpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT token in the format: your-token-here (without 'Bearer' prefix)"
        };

        // Apply security requirement globally
        document.SecurityRequirements ??= new List<Microsoft.OpenApi.Models.OpenApiSecurityRequirement>();
        document.SecurityRequirements.Add(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        return Task.CompletedTask;
    });
});

// Add database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("5.7.0-mysql")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add JWT Service
builder.Services.AddScoped<QUANTM.Services.Auth.JwtService>();

// Add Audit Service
builder.Services.AddScoped<QUANTM.Services.Auth.AuditService>();
builder.Services.AddHttpContextAccessor();

// Add Password Policy Service
builder.Services.AddScoped<QUANTM.Services.Auth.PasswordPolicyService>();

// Add Email Service
builder.Services.AddScoped<QUANTM.Services.Auth.EmailService>();

// Add Encryption Service
builder.Services.AddScoped<QUANTM.Services.Auth.EncryptionService>();

// Add Document Service
builder.Services.AddScoped<QUANTM.Services.Common.IDocumentService, QUANTM.Services.Common.DocumentService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
        )
    };

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // First, try to get token from Authorization header (for Swagger/Postman)
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                context.Token = authHeader.Substring("Bearer ".Length).Trim();
            }
            // If not found in header, try to get from cookie (for browser)
            else if (context.Request.Cookies.ContainsKey("X-Access-Token"))
            {
                context.Token = context.Request.Cookies["X-Access-Token"];
            }

            return Task.CompletedTask;
        }
    };
});

// Configure Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<AspNetCoreRateLimit.IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.ClientIdHeader = "X-ClientId";
    options.GeneralRules = new List<AspNetCoreRateLimit.RateLimitRule>
    {
        new AspNetCoreRateLimit.RateLimitRule
        {
            Endpoint = "POST:/api/auth/login",
            Period = "1m",
            Limit = 5 // 5 login attempts per minute
        },
        new AspNetCoreRateLimit.RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100 // 100 requests per minute for all other endpoints
        }
    };
});

builder.Services.AddSingleton<AspNetCoreRateLimit.IIpPolicyStore, AspNetCoreRateLimit.MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<AspNetCoreRateLimit.IRateLimitCounterStore, AspNetCoreRateLimit.MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<AspNetCoreRateLimit.IRateLimitConfiguration, AspNetCoreRateLimit.RateLimitConfiguration>();
builder.Services.AddSingleton<AspNetCoreRateLimit.IProcessingStrategy, AspNetCoreRateLimit.AsyncKeyLockProcessingStrategy>();

var app = builder.Build();

// Add logging middleware
app.UseMiddleware<QUANTM.Middleware.LoggingMiddleware>();

// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
//     app.UseSwaggerUI(options =>
//     {
//         options.SwaggerEndpoint("/openapi/v1.json", "v1");
//     });
// }

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
});


// display "QUANTM API is running" when route to /
app.MapGet("/", () => "QUANTM API is running");

if (!app.Environment.IsDevelopment())
{
    // app.UseHttpsRedirection(); 
}

// Add IP Rate Limiting
app.UseMiddleware<AspNetCoreRateLimit.IpRateLimitMiddleware>();

app.UseCors("BankingPolicy");
app.UseMiddleware<QUANTM.Middleware.SecurityHeadersMiddleware>();

app.UseAuthentication();
app.UseMiddleware<QUANTM.Middleware.SessionValidationMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();