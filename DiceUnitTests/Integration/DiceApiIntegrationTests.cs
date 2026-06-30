using Dice.BusinessLogic;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Numerics;
using System.Text.Json;
using Xunit;

namespace DiceUnitTests.Integration;

public class DiceApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DiceApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "GET /api/dice 1d6 vs 1d6 returns symmetric win/lose and 1/6 tie")]
    public async Task Dice_EqualDice_ReturnsExpectedProbabilities()
    {
        var response = await _client.GetAsync("/api/dice?dice1=1&dice2=1&sides=6");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;

        Assert.Equal(root.GetProperty("win").GetDouble(), root.GetProperty("lose").GetDouble());
        Assert.Equal(0.1667, root.GetProperty("tie").GetDouble(), 4);
    }

    [Fact(DisplayName = "GET /api/dice/WaysToRoll(7,2,6) returns 6")]
    public async Task WaysToRoll_KnownInput_ReturnsExpectedCount()
    {
        var response = await _client.GetAsync("/api/dice/WaysToRoll?targetSum=7&dice=2&sides=6");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = (await response.Content.ReadAsStringAsync()).Trim();
        Assert.Equal("6", body);
    }

    [Fact(DisplayName = "GET /api/dice with out-of-range dice returns 400 ProblemDetails")]
    public async Task Dice_OutOfRangeInput_Returns400()
    {
        var response = await _client.GetAsync("/api/dice?dice1=99&dice2=1&sides=6");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact(DisplayName = "GET /api/dice/WaysToRoll serializes large counts as an exact, unquoted JSON number")]
    public async Task WaysToRoll_LargeCount_SerializesExactlyAsNumber()
    {
        // The count for this roll exceeds 2^53, so a double-based pipeline would round it.
        var response = await _client.GetAsync("/api/dice/WaysToRoll?targetSum=505&dice=10&sides=100");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = (await response.Content.ReadAsStringAsync()).Trim();

        // Unquoted integer literal: no quotes, no decimal point, no exponent.
        Assert.DoesNotContain('"', body);
        Assert.DoesNotContain('.', body);
        Assert.DoesNotContain('E', body.ToUpperInvariant());

        var expected = new MathHelper().WaysToRoll(505, 10, 100);
        Assert.True(expected > new BigInteger(9_007_199_254_740_992L)); // sanity: > 2^53
        Assert.Equal(expected, BigInteger.Parse(body));
    }
}
