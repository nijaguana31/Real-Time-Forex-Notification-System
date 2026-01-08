using Forex.API.Hubs;
using Forex.Application.Interfaces;
using Forex.Infrastructure.BackgroundServices;
using Forex.Infrastructure.Persistence.DbContext;
using Forex.Infrastructure.Repositories;
using Forex.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Formatting.Compact;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// DbContext (PostgreSQL)
builder.Services.AddDbContext<ForexDbContext>(options =>
    options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(10);
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    })
);



// Repositories
builder.Services.AddScoped<IPriceTickRepository, PriceTickRepository>();
builder.Services.AddScoped<ISubscriptionAuditRepository, SubscriptionAuditRepository>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Forex.Application.DTOs.PriceTickDto).Assembly));
builder.Services.AddSignalR();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        // IMPORTANT: Allow JWT in SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/forexHub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IFinnhubClient, FinnhubClient>();

builder.Services.AddHostedService<ForexPriceFeedService>();
builder.Services.AddHostedService<PriceBroadcastService>();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()          // REQUIRED
    .Enrich.WithProperty("MachineName", Environment.MachineName)
    .Enrich.WithProperty("ThreadId", Thread.CurrentThread.ManagedThreadId)

    // Structured JSON logs
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .WriteTo.File(
        new RenderedCompactJsonFormatter(),
        "Logs/forex-log-.json",
        rollingInterval: RollingInterval.Day)

    .CreateLogger();



builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSignalR", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true); // allow any origin (dev only)
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowSignalR");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ForexHub>("/forexHub");

app.Run();
Log.Information("Forex Notification API started successfully");
