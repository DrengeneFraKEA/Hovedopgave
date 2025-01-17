using DotNetEnv;
using DotNetEnv.Configuration;
using Hovedopgave.Server.Database;
using Hovedopgave.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "FrontEndUI", policy =>
    {
        policy.WithOrigins("https://localhost:4200/").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    });
});

// Register PostgreSQL as a singleton
builder.Services.AddSingleton(sp => new PostgreSQL()); // False when using hosted DB

// Register ApplicationDbContext with the PostgreSQL connection string
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    var postgreSql = serviceProvider.GetRequiredService<PostgreSQL>();
    options.UseNpgsql(PostgreSQL.GetConnectionString());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Env.GetString("ISSUER"),
            ValidAudience = Env.GetString("AUDIENCE"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("JWT_TOKEN")))
        };
    });


builder.Services.AddControllers();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IGraphService, GraphService>();

var config = new ConfigurationBuilder().AddDotNetEnv("env.env", LoadOptions.TraversePath()).Build();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("FrontEndUI");
app.UseAuthorization();
app.MapControllers();

app.Run();
