using BusinessLayer.Services;
using RepositoryLayer.Interfaces;
using RepositoryLayer.DatabaseRepository;
using QuantityMeasurementApp.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using BusinessLayer.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddScoped<IQuantityMeasurementRepository, QuantityMeasurementDatabaseRepository>();
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>();
builder.Services.AddScoped<GlobalExceptionHandler>();
builder.Services.AddScoped<IUserRepository, UserDatabaseRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "defaultSecretKeyXYZ123");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// CORS: allow frontend on Live Server to call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500",
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "https://tusharpatelgithub.github.io",
                "null")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Quantity Measurement API",
        Version = "v1",
        Description = "REST API for performing quantity measurement operations"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your valid token in the text input below.\r\n\r\nExample: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors("AllowFrontend");
app.UseRouting();
app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1"));

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

// Auto-create tables on startup
using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var connStr = config.GetConnectionString("DefaultConnection");
    using var conn = new Npgsql.NpgsqlConnection(connStr);
    conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS ""Measurements"" (
            ""Id""              SERIAL PRIMARY KEY,
            ""MeasurementType"" VARCHAR(100)   NOT NULL,
            ""OperationType""   VARCHAR(100)   NOT NULL,
            ""Value1""          DOUBLE PRECISION NOT NULL,
            ""Value2""          DOUBLE PRECISION NOT NULL,
            ""Result""          DOUBLE PRECISION NOT NULL,
            ""Unit""            VARCHAR(50)    NOT NULL,
            ""CreatedAt""       TIMESTAMP      NOT NULL DEFAULT NOW(),
            ""IsError""         BOOLEAN        NOT NULL DEFAULT FALSE,
            ""ErrorMessage""    VARCHAR(500)   NULL
        );
        CREATE TABLE IF NOT EXISTS ""Users"" (
            ""Id""           SERIAL          PRIMARY KEY,
            ""FullName""     VARCHAR(100)    NOT NULL DEFAULT '',
            ""Email""        VARCHAR(256)    NOT NULL UNIQUE,
            ""PasswordHash"" TEXT            NOT NULL DEFAULT '',
            ""MobileNumber"" VARCHAR(10)     NOT NULL DEFAULT '',
            ""GoogleId""     VARCHAR(256)    NULL
        );";
    cmd.ExecuteNonQuery();
}

app.Run();

public partial class Program { }


