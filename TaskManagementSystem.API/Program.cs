using TaskManagementSystem.API;
using TaskManagementSystem.API.Middlewares;
using TaskManagementSystem.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.ConfigureAuthorizationPolicies();
builder.Services.ConfigureSwagger();
builder.Services.RegisterServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1"));
}

app.UseHttpsRedirection();

// Add the exception handler middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Authentication & Authorization middleware - ORDER IS IMPORTANT
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed initial roles and users
await DataSeeder.SeedRolesAndUsersAsync(app.Services);

app.Run();