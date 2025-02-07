namespace Bet.Models;

public class Games
{
    public string? TeamFirst { get; private set; }
    public string? TeamFirstLogoSrc { get; private set; }
    public string? TeamSecond { get; private set; }
    public string? TeamSecondLogoSrc { get; private set; }
    public string? Time { get; set; }

    public Games(
        string teamFirst,
        string teamSecond,
        string? time,
        string? teamFirstLogoSrc,
        string? teamSecondLogoSrc
    )
    {
        TeamFirst = teamFirst;
        TeamSecond = teamSecond;
        Time = time;
        TeamFirstLogoSrc = teamFirstLogoSrc;
        TeamSecondLogoSrc = teamSecondLogoSrc;
    }
}
