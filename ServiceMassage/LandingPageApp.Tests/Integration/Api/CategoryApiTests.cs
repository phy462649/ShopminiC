using System.Net;
using System.Net.Http.Json;
using LandingPageApp.Application.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LandingPageApp.Tests.Integration.Api;

/// <summary>
/// Integration tests cho Category API endpoints.
/// Test full flow từ HTTP request đến database.
/// Đây là layer cao nhất - test toàn bộ pipeline.
/// </summary>
public class CategoryApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CategoryApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region GET /api/Category Tests

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/Category");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfCategories()
    {
        // Act
        var response = await _client.GetAsync("/api/Category");
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>();

        // Assert
        categories.Should().NotBeNull();
    }

    #endregion

    #region GET /api/Category/{id} Tests

    [Fact]
    public async Task GetById_WhenNotExists_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/Category/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    // Note: POST, PUT, DELETE cần authentication
    // Để test đầy đủ, cần setup test authentication
}
