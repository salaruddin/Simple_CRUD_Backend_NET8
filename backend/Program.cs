using backend.Core.AutoMapper;
using backend.Core.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

//configuring serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Load configuration from appsettings
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("localdb")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

Log.Information("Application Started");
app.Run();
