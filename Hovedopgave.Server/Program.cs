var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "FrontEndUI", policy =>
    {
        policy.WithOrigins("https://localhost:4200/").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    });
});

builder.Services.AddControllers();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("FrontEndUI");

app.UseAuthorization();

app.MapControllers();

app.Run();
