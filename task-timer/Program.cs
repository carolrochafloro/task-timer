
using Microsoft.EntityFrameworkCore;
using task_timer.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using task_timer.Filters;
using System.Text.Json.Serialization;
using task_timer.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers( options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));

}).AddJsonOptions( options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Enable OpenAPI

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Enable JWT

string issuer = "";
string audience = "";
string secretKey = "";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["jwtSettings:issuer"],
            //ValidAudience = builder.Configuration["jwtSettings:audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtSettings:secretKey"]))
        };
    });

builder.Services.AddAuthorization();

// Add user secrets and build connection string

builder.Configuration.AddUserSecrets<Program>();

string? host = builder.Configuration["database:host"];

int? port = int.Parse(builder.Configuration["database:port"]);

string? database = builder.Configuration["database:database"];

string? username = builder.Configuration["database:username"];

string? password = builder.Configuration["database:password"];


string connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

// Use connection string and register the dbcontext in the DI container
builder.Services.AddDbContext<TTDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// 

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ITasksRepository, TasksRepository>();
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Using JWT
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

/*
 * Task timer: criar usuário, editar usuário, deletar usuário, login, cadastrar tipo de tarefa,
 * iniciar tarefa (timestamp), finalizar tarefa (timestamp),
 * salvar tarefa, listar tarefas (com tempo médio e data da última realização)
 * com filtro por intervalo, deletar tarefa, editar tarefa.
 */