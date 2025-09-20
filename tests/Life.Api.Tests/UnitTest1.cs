using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Life.Api.Tests.Infrastructure;
using Life.Application.Board.Contracts;
using Life.Infrastructure.Data;

namespace Life.Api.Tests;

[TestFixture]
public class BoardsControllerIntegrationTests
{
    private LifeApiTestFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = LifeApiTestFactory.Create();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _factory.CleanupDatabaseAsync();
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    [Test]
    public async Task Upload_ValidBoard_ReturnsCreatedBoardId()
    {
        // Arrange
        var uploadRequest = new UploadRequest(
            Width: 5,
            Height: 5,
            Alive: new[] { new[] { 1, 1 }, new[] { 2, 2 }, new[] { 3, 3 } }
        );

        var json = JsonSerializer.Serialize(uploadRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/boards/upload", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var boardId = await response.Content.ReadAsStringAsync();
        Assert.That(boardId, Is.Not.Null.And.Not.Empty);
        Assert.That(Guid.TryParse(boardId.Replace("-", ""), out _), Is.True, "Board ID should be a valid GUID");

        // Verify data was saved to database
        await VerifyBoardExistsInDatabase(boardId, uploadRequest.Width, uploadRequest.Height);
    }

    [Test]
    public async Task Upload_InvalidBoard_ReturnsBadRequest()
    {
        // Arrange - Invalid board with negative dimensions
        var uploadRequest = new UploadRequest(
            Width: -1,
            Height: 5,
            Alive: new[] { new[] { 1, 1 } }
        );

        var json = JsonSerializer.Serialize(uploadRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/boards/upload", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Next_ValidBoardId_ReturnsNextGeneration()
    {
        // Arrange - First create a board
        var boardId = await CreateTestBoard();
        var nextRequest = new NextRequest(boardId);

        var json = JsonSerializer.Serialize(nextRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/boards/next", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var responseJson = await response.Content.ReadAsStringAsync();
        var boardDto = JsonSerializer.Deserialize<BoardDto>(responseJson, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });

        Assert.That(boardDto, Is.Not.Null);
        Assert.That(boardDto.Width, Is.EqualTo(5));
        Assert.That(boardDto.Height, Is.EqualTo(5));
        Assert.That(boardDto.Alive, Is.Not.Null);

        // Verify next generation was saved to database
        await VerifySnapshotExistsInDatabase(boardId, generation: 1);
    }

    [Test]
    public async Task Next_NonExistentBoardId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentBoardId = Guid.NewGuid().ToString("N");
        var nextRequest = new NextRequest(nonExistentBoardId);

        var json = JsonSerializer.Serialize(nextRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/boards/next", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task NAhead_ValidBoardIdAndGenerations_ReturnsCorrectGeneration()
    {
        // Arrange
        var boardId = await CreateTestBoard();
        var nAheadRequest = new NAheadRequest(boardId, 3);

        var json = JsonSerializer.Serialize(nAheadRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/boards/n-ahead", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var responseJson = await response.Content.ReadAsStringAsync();
        var boardDto = JsonSerializer.Deserialize<BoardDto>(responseJson, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });

        Assert.That(boardDto, Is.Not.Null);
        Assert.That(boardDto.Width, Is.EqualTo(5));
        Assert.That(boardDto.Height, Is.EqualTo(5));

        // Verify the 3rd generation was saved to database
        await VerifySnapshotExistsInDatabase(boardId, generation: 3);
    }

    [Test]
    public async Task NAhead_ZeroGenerations_ReturnsBadRequest()
    {
        // Arrange
        var boardId = await CreateTestBoard();
        var nAheadRequest = new NAheadRequest(boardId, 0);

        var json = JsonSerializer.Serialize(nAheadRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/boards/n-ahead", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Final_ValidBoardId_ReturnsFinalState()
    {
        // Arrange
        var boardId = await CreateTestBoard();
        var finalRequest = new FinalRequest(boardId);

        var json = JsonSerializer.Serialize(finalRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/boards/final", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var responseJson = await response.Content.ReadAsStringAsync();
        var finalDto = JsonSerializer.Deserialize<FinalDto>(responseJson, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });

        Assert.That(finalDto, Is.Not.Null);
        Assert.That(finalDto.Board, Is.Not.Null);
        Assert.That(finalDto.Iterations, Is.GreaterThan(0));
        Assert.That(finalDto.Stable || finalDto.Cyclic, Is.True, "Board should reach either stable or cyclic state");
    }

    [Test]
    public async Task Final_NonExistentBoardId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentBoardId = Guid.NewGuid().ToString("N");
        var finalRequest = new FinalRequest(nonExistentBoardId);

        var json = JsonSerializer.Serialize(finalRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/boards/final", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task MultipleRequests_WithSameBoard_ShouldIsolateData()
    {
        // Arrange - Create two different boards
        var boardId1 = await CreateTestBoard();
        var boardId2 = await CreateTestBoard(pattern: new[] { new[] { 0, 0 }, new[] { 1, 1 }, new[] { 2, 2 } });

        // Act - Process both boards simultaneously
        var next1Task = ProcessNextGeneration(boardId1);
        var next2Task = ProcessNextGeneration(boardId2);

        var results = await Task.WhenAll(next1Task, next2Task);

        // Assert - Both should succeed independently
        Assert.That(results[0], Is.EqualTo(HttpStatusCode.OK));
        Assert.That(results[1], Is.EqualTo(HttpStatusCode.OK));

        // Verify both boards exist in database with their respective generations
        await VerifySnapshotExistsInDatabase(boardId1, generation: 1);
        await VerifySnapshotExistsInDatabase(boardId2, generation: 1);
    }

    #region Helper Methods

    private async Task<string> CreateTestBoard(int[][]? pattern = null)
    {
        pattern ??= new[] { new[] { 1, 1 }, new[] { 2, 2 }, new[] { 3, 3 } };
        
        var uploadRequest = new UploadRequest(
            Width: 5,
            Height: 5,
            Alive: pattern
        );

        var json = JsonSerializer.Serialize(uploadRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/v1/boards/upload", content);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        return await response.Content.ReadAsStringAsync();
    }

    private async Task<HttpStatusCode> ProcessNextGeneration(string boardId)
    {
        var nextRequest = new NextRequest(boardId);
        var json = JsonSerializer.Serialize(nextRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/v1/boards/next", content);
        return response.StatusCode;
    }

    private async Task VerifyBoardExistsInDatabase(string boardId, int expectedWidth, int expectedHeight)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LifeDbContext>();
        var board = await context.Boards.FindAsync(boardId);
        
        Assert.That(board, Is.Not.Null, $"Board with ID {boardId} should exist in database");
        Assert.That(board.Width, Is.EqualTo(expectedWidth));
        Assert.That(board.Height, Is.EqualTo(expectedHeight));
    }

    private async Task VerifySnapshotExistsInDatabase(string boardId, long generation)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LifeDbContext>();
        
        var snapshot = await context.Snapshots
            .FirstOrDefaultAsync(s => s.BoardId == boardId && s.Generation == generation);
        
        Assert.That(snapshot, Is.Not.Null, $"Snapshot for board {boardId} generation {generation} should exist in database");
    }

    #endregion
}
