using MagicVilla_VillaAPI.Config;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//        .WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();

//builder.Host.UseSerilog();


// Add services to the container.
builder.Services.AddDbContext < ApplicationDbContext>(option => 
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));

});// Dependency Injection for DB Context


builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IVillaRepository, VillaRepository>(); // Dependency Injection for Villa Repository
builder.Services.AddAutoMapper(typeof(MappingConfig)); // Dependency Injection for AutoMapper

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ILogging,Logging>(); // Dependency Injection for custom logging

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
