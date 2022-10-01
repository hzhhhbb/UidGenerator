
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//string connectionString = "Server=localhost;Port=3306;Database=uid;Uid=root;Pwd=123456;";
//builder.Services.AddCachedUidGeneratorService(AssignWorkIdScheme.MySql, connectionString, new CachedUidGeneratorOptions());
builder.Services.AddDefaultUidGeneratorService();

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