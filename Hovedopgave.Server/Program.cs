using Hovedopgave.Server.Database;
using Hovedopgave.Server.Services;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddSingleton(sp => new PostgreSQL(uselocaldb: true)); // False when using hosted DB

// Register ApplicationDbContext with the PostgreSQL connection string
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    var postgreSql = serviceProvider.GetRequiredService<PostgreSQL>();
    options.UseNpgsql(postgreSql.connectionstring);
});

builder.Services.AddControllers();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("FrontEndUI");
app.UseAuthorization();
app.MapControllers();

app.Run();
