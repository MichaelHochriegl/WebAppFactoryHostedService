using System.Net;
using System.Net.Http.Json;
using Api.Data;
using FluentAssertions;

namespace Api.Tests.Integration;

public class UnitTest1 : IClassFixture<MovieWebApplicationFactory>
{
    private readonly MovieWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UnitTest1(MovieWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Test1()
    {
        // Arrange
        
        // Act
        var result = await _client.GetAsync("api/movies");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await result.Content.ReadFromJsonAsync<IEnumerable<Movie>>();
        content.Should().NotBeNull();
        content!.Count().Should().Be(2);
    }
}