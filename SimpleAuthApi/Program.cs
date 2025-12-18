using Application.Interfaces;
using Application.Queues;
using Application.Services;
using Contracts.Requests;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using Persistence;
using SimpleAuthApi.BackgroundServices;
using SimpleAuthApi.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var services = builder.Services;

services.AddDbContext<AppDbContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(AppDbContext)));
});

services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

services.AddSingleton<FileLogQueue>();
services.AddHostedService<FileLogWriterHostedService>();

services.AddSingleton<IUserService, UserService>();
services.AddScoped<IValidator<AuthRequest>, AuthRequestValidator>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

services.AddControllers();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    MigrateDatabase(app);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseSession();

app.MapControllers();

app.Run();

static void MigrateDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}