
using Microsoft.EntityFrameworkCore;
using task_timer.Context;
using Microsoft.Extensions.Configuration.UserSecrets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Enable OpenAPI

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

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

/*
 * Task timer: criar usuário, editar usuário, deletar usuário, login, cadastrar tipo de tarefa,
 * iniciar tarefa (timestamp), finalizar tarefa (timestamp),
 * salvar tarefa, listar tarefas (com tempo médio e data da última realização)
 * com filtro por intervalo, deletar tarefa, editar tarefa.
 */