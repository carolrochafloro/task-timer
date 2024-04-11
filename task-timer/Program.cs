// Create builder

using Microsoft.EntityFrameworkCore;
using task_timer.Context;

var builder = WebApplication.CreateBuilder(args);

// Add user secrets and build connection string

builder.Configuration.AddUserSecrets<Program>();

string? host = builder.Configuration["database:host"];

int? port = int.Parse(builder.Configuration["database:port"]);

string? database = builder.Configuration["database:database"];

string? username = builder.Configuration["database:username"];

string? password = builder.Configuration["database:password"];


string connectionString = $"Host={host};Port={port};Database={database};" +
                          $"Username={username};Password={password}";

// Use connection string and register the dbcontext in the DI container
builder.Services.AddDbContext<TTDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Add services to the container

builder.Services.AddControllers();

// Enable OpenAPI

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapControllers();

app.Run();

/*
 * Task timer: criar usu�rio, editar usu�rio, deletar usu�rio, login, cadastrar tipo de tarefa,
 * iniciar tarefa (timestamp), finalizar tarefa (timestamp),
 * salvar tarefa, listar tarefas (com tempo m�dio e data da �ltima realiza��o)
 * com filtro por intervalo, deletar tarefa, editar tarefa.
 */