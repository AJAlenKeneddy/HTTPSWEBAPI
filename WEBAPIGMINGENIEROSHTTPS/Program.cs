using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AppWebApiGMINGENIEROS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AppWebApiGMINGENIEROS.Custom;

using System.Text;
using Humanizer;


var builder = WebApplication.CreateBuilder(args);
var cadcn = builder.Configuration.GetConnectionString("cn1");

builder.Services.AddDbContext<DatadecomprasgmContext>(
    opt => opt.UseSqlServer(cadcn));
builder.Services.AddSingleton<Utilidades>();


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

    });


builder.Services.AddSwaggerGen(c =>
{
    c.IgnoreObsoleteActions();
    c.IgnoreObsoleteProperties();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Consultas GM INGENIEROS API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese su Token que ha sido envia a su correo si logro el login",
        Name = "Autorizacion",
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
