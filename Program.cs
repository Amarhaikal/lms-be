using LMS;
using LMS.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Add logging middleware
app.UseMiddleware<LMS.Middleware.LoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

// display "LMS API is running" when route to /
app.MapGet("/", () => "LMS API is running");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();