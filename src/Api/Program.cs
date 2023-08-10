using Api.Data;
using Api.HostedServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MovieDbContext>(optionsBuilder =>
{
    var conn = builder.Configuration.GetConnectionString("MovieDb");
    optionsBuilder.UseNpgsql(conn);
});

builder.Services.AddHostedService<MovieHostedService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/api/movies", (MovieDbContext dbContext) => dbContext.Movies.ToListAsync());

app.Run();