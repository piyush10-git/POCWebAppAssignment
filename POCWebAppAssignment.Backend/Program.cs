//using POCWebAppAssignment.Orchestration.Interfaces;
using POCWebAppAssignment.Orchestration.Services;
using POCWebAppAssignment.Interfaces;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Repository.Repositories;
using POCWebAppAssignment.Repository.RunStoredProcedures;
using Microsoft.AspNetCore.Authentication;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
.WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authorization and Authentication DI
builder.Services.AddTransient<IAuthStoredProcedures, AuthStoredProcedures>();
builder.Services.AddTransient<IAuthRepository, AuthRepository>();
builder.Services.AddTransient<IAuthService, AuthService>();

// Registering dependencies
builder.Services.AddTransient<IRunStoredProceduresNormalizedTable, RunStoredProceduresNormalizedTable>();
builder.Services.AddTransient<IResourceRepository, ResourceRepository>();
builder.Services.AddTransient<IResourceService, ResourceService>();

// JWT setting from configuration
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

//builder.Services.AddAuthentication(option =>
//{
//    option.DefaultAuthenticateScheme
//})


builder.Services.AddControllers();
builder.Host.UseSerilog();

var app = builder.Build();

app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
