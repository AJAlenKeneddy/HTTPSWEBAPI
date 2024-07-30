using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WEBAPIGMINGENIEROSHTTPS.Custom;
using WEBAPIGMINGENIEROSHTTPS.Models;
using WEBAPIGMINGENIEROSHTTPS.Models.Services;
using WEBAPIGMINGENIEROSHTTPS.Controllers;

var builder = WebApplication.CreateBuilder(args);
var cadcn = builder.Configuration.GetConnectionString("cn1");

// Configuración de servicios
builder.Services.AddDbContext<DatadecomprasgmContext>(opt => opt.UseSqlServer(cadcn));

// Registro de servicios como singleton o scoped
builder.Services.AddSingleton<Utilidades>();

builder.Services.AddSingleton(new EmailService(
    smtpServer: "smtp.gmail.com",
    smtpPort: 587,
    smtpUser: "alenaguilar24@gmail.com",
    smtpPass: "krvd ajsr ruuf fwgj"
));

// Registrar el controlador como un servicio
builder.Services.AddScoped<AccesoController>();

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Configuraciones adicionales de JSON (si es necesario)
    });

builder.Services.AddSwaggerGen(c =>
{
    c.IgnoreObsoleteActions();
    c.IgnoreObsoleteProperties();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Consultas GM INGENIEROS API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese su Token que ha sido enviado a su correo si logró el login",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Añadir un requisito de seguridad para JWT
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API V1");
    c.DefaultModelsExpandDepth(-1); // Oculta la sección de modelos (schemas)
    c.DefaultModelExpandDepth(-1);
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
