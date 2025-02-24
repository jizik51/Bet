using System.Text.Json.Serialization;

namespace Bet.Models;

public class GameWrapper
{
    // public Dictionary<string, Games> Games { get; set; }
    [JsonPropertyName("games")]
    public List<Games> Games { get; set; } = new();
}

public class Games
{
    public int GameId { get; set; }
    public int FirstId { get; set; }
    public int SecondId { get; set; }
    public string? TeamFirst { get; set; }
    public string? TeamFirstLogoSrc { get; set; }
    public string? TeamSecond { get; set; }
    public string? TeamSecondLogoSrc { get; set; }
    public string? Time { get; set; }
}
