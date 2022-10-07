
using System.Collections.Concurrent;
using Vincent.UidGenerator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string connectionString = "Server=localhost;Port=3306;Database=uid;Uid=root;Pwd=123456;";
//builder.Services.AddCachedUidGeneratorService(AssignWorkIdScheme.MySql, connectionString);
builder.Services.AddCachedUidGenerator(options => { });
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

app.Use(async (context, next) =>
{
    var uidGenerator = context.RequestServices.GetRequiredService<IUidGenerator>();
    await context.Response.WriteAsync(uidGenerator.GetUid().ToString());
    await context.Response.CompleteAsync();
    await next();
});    
app.MapControllers();

app.Run();