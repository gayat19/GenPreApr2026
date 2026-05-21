using BankingAPI.Contexts;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Repositories;
using BankingAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Contexts
builder.Services.AddDbContext<BankingContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
#endregion

#region Repositories
builder.Services.AddScoped<IRepository<string, Account>, Repository<string,Account>>();
builder.Services.AddScoped<IRepository<int, Customer>, Repository<int, Customer>>();
builder.Services.AddScoped<IRepository<string, User>, Repository<string, User>>();
#endregion


#region Services
builder.Services.AddScoped<ICustomerInteract, CustomerService>();
builder.Services.AddScoped<IAuthenticationService, CustomerService>();
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
