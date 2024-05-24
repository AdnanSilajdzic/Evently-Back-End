using Microsoft.EntityFrameworkCore;
using dotenv.net;
using EventlyBackEnd.Models.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connection = Environment.GetEnvironmentVariable("AzureSqlServerConnection");

builder.Services.AddControllers();
builder.Services.AddDbContext<EventlyDbContext>(options =>
    options.UseSqlServer(connection));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

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
