using Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace Api.Tests.Integration;

public class MovieWebApplicationFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("testing-db")
        .WithUsername("testuser")
        .WithPassword("testpassword")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
        });

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.Single(
                d => d.ServiceType ==
                     typeof(DbContextOptions<MovieDbContext>));
            
            services.Remove(dbContextDescriptor);
            services.AddDbContext<MovieDbContext>(opt =>
            {
                var conn = _dbContainer.GetConnectionString();
                opt.UseNpgsql(conn);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
        var connString = dbContext.Database.GetConnectionString();
        await dbContext.Database.MigrateAsync().ConfigureAwait(false);

        dbContext.Movies.Add(new Movie(){Name = "Ace Ventura"});
        dbContext.Movies.Add(new Movie(){Name = "Hot Shots"});

        await dbContext.SaveChangesAsync().ConfigureAwait(false);

    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}