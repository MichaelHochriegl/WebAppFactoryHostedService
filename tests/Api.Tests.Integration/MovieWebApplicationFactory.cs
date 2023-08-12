using Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
        builder.ConfigureLogging(loggingBuilder => { loggingBuilder.ClearProviders(); });

        builder.ConfigureTestServices(services =>
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

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
            dbContext.Database.Migrate();
        });

    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}