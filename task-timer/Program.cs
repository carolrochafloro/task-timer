using Microsoft.EntityFrameworkCore;
using task_timer.Context;
using task_timer.Filters;
using System.Text.Json.Serialization;
using task_timer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using task_timer.Models;
using task_timer.Serices;
using task_timer.Services;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));

}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


// Enable OpenAPI

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>().
                             AddEntityFrameworkStores<TTDbContext>().
                             AddDefaultTokenProviders();

// Enable JWT
string secretKey = builder.Configuration["JWT:SecretKey"]
                   ?? throw new ArgumentException("Unable to autheticate. Check secret key.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; //change when in production
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
    };
});

builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(typeof(Program));

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

builder.Services.AddScoped<IAppTasksRepository, AppTasksRepository>();
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}

        }
    });
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
