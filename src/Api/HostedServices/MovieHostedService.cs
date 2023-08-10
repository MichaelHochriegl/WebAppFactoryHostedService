using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.HostedServices;

public class MovieHostedService : IHostedService
{
    private readonly ILogger<MovieHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MovieHostedService(ILogger<MovieHostedService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
        var moviesCount = await dbContext.Movies.CountAsync(cancellationToken: cancellationToken);
        _logger.LogInformation("Movie count: {movieCount}", moviesCount);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}