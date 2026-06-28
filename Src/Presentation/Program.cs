using Domain.Interface;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Service;
using Infrastructure;
using Infrastructure.Repositories;
using Presentation.Authorization;
using Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Polly;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN DE SERVICIOS ---

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresá el token JWT. No hace falta escribir 'Bearer', Swagger lo agrega solo."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", document), [] }
    });
});

// 1. Capa de Infraestructura (Persistencia)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 2. Repositorios
// Es vital registrar IUserRepository porque UserService lo pide en su constructor
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<ISysAdminRepository, SysAdminRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IInscriptionRepository, InscriptionRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<ISysAdminService, SysAdminService>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();


// 3. Servicios de Aplicación (Lógica de Negocio)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ISysAdminService, SysAdminService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IInscriptionService, InscriptionService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//Servicios de utilidad
builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

builder.Services.AddHostedService<SubscriptionBackgroundService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.SoloAdmin, policy => policy.RequireRole("Admin"));
    options.AddPolicy(Policies.SoloClient, policy => policy.RequireRole("Client"));
    options.AddPolicy(Policies.SoloSysAdmin, policy => policy.RequireRole("SysAdmin"));
    options.AddPolicy(Policies.AdminOSysAdmin, policy => policy.RequireRole("Admin", "SysAdmin"));
});


builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

builder.Services.AddHttpClient<IMercadoPagoService, MercadoPagoService>(client =>
{
    client.BaseAddress = new Uri("https://api.mercadopago.com/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
}).AddResilienceHandler("mercadopago", builder =>
{
    // Retry (3 intentos, backoff exponencial)
    builder.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(2),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true
    });

    // Circuit Breaker
    builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
    {
        SamplingDuration = TimeSpan.FromSeconds(30),
        FailureRatio = 0.1,           // 10% de fallos abre el circuito
        MinimumThroughput = 100,      // mínimo 100 requests para evaluar
        BreakDuration = TimeSpan.FromSeconds(5)
    });

    // Timeout por intento
    builder.AddTimeout(TimeSpan.FromSeconds(30));
});




// --- PIPELINE DE LA APLICACIÓN ---

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Sembrar la base de datos con un usuario sysadmin por defecto

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider
    .GetRequiredService<DatabaseSeeder>();


    await seeder.SeedAsync();
}


//app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();