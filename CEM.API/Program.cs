using CEM.Cache;
using CEM.Data;
using CEM.Model;
using CEM.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using StackExchange.Redis;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connString = builder.Configuration.GetConnectionString("dbConn");
builder.Services.AddEndpointsApiExplorer();
//MySQL DB Injection
builder.Services.AddDbContext<CEMDbContext>(options =>
      options.UseMySql(connString,ServerVersion.AutoDetect(connString)));
var redisConnString = builder.Configuration.GetConnectionString("Redis");
var exchangeRatesURL = builder.Configuration.GetSection("ExchangeRatesURL").Value;
var exchangeRatesHistory = builder.Configuration.GetSection("ExchangeRatesHistoricalURL").Value;
var timeOut = builder.Configuration.GetValue<int>("HistoricalRefreshTime");
//Redis Cache Injection
builder.Services.AddScoped<ICacheRepository, CacheRepository>(opt =>
{
    var config = opt.GetRequiredService<IConfiguration>();
    return new CacheRepository(redisConnString, exchangeRatesURL);
});

//Configure custom login via Serilog extention
builder.Host.UseSerilog(
    (context,configuration)=> configuration.ReadFrom.Configuration(context.Configuration));

//Historical Rates update background service;
builder.Services.AddHostedService<CacheHistoryService>(opt => {
    var delayTime = timeOut;
    var scope = opt.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<CEMDbContext>();
    var exchangeHistoryManager = opt.GetRequiredService<IManageCurrencyExchange>();
    return new CacheHistoryService(context, exchangeHistoryManager, delayTime, exchangeRatesHistory);
});
builder.Services.AddSingleton<IManageCurrencyExchange, ManageCurrencyExchange>();


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
