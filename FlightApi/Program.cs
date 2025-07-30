using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FlightData.Data;
using FlightApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Añadimos swagger para tener documentacion del proyecto
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Flight API",
        Version = "v1",
        Description = "Api para gestionar vuelos, pasajeros y reservas"
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



// Cadena de conexión SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration
       .GetConnectionString("DefaultConnection") ??
       "Data Source=FlightDb.db")
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight API v1");
        c.RoutePrefix = string.Empty; // sirve Swagger en la raíz
    });
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
