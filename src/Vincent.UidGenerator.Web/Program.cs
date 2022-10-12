
using System.Collections.Concurrent;
using Vincent.UidGenerator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string connectionString = "Server=localhost;Port=3306;Database=uid;Uid=root;Pwd=1;";
string connectionStringForSQLServer = "Server=localhost;Database=uid;User Id=sa;Password=1@hH;TrustServerCertificate=True";

builder.Services.AddCachedUidGenerator(options => { }).AddMySQLWorker(connectionString);
//builder.Services.AddCachedUidGenerator(options => { }).AddSQLServerWorker(connectionStringForSQLServer);
//builder.Services.AddDefaultUidGeneratorService();
builder.Services.AddHealthChecks();
var app = builder.Build();
var buffer = new ConcurrentQueue<long>();

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