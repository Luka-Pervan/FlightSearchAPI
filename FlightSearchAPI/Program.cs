using FlightSearchAPI.Services;
using Microsoft.AspNetCore.Hosting;

/****************************************************
 *                FlightSearch API                  *
 **************************************************** 
 * File:    Program
 * Date:    09/09/2024
 * Author:  Luka Pervan
 * Summary: An API made that is used as a middleware 
 * in communication between a ReactJS frontend and 
 * Amadeus API for flights informations.
****************************************************/

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<FlightSearchService>();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddMemoryCache();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
