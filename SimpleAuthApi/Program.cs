using Application.Interfaces;
using Application.Services;
using FluentValidation;
using SimpleAuthApi.Requests;
using SimpleAuthApi.Validators;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseSession();

app.MapControllers();

app.Run();