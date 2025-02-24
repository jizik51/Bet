using System.IO;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json;
using Bet.Models;
using System.Diagnostics;
using Services;
using MvcBet.Data;

namespace Bet.Controllers;

public class HomeController : Controller
{
    private readonly FindGamesController _findGamesService;
    private readonly StatsServices _statsServices;
    private readonly MvcBetContext _context;

    public HomeController(
        FindGamesController findGames,
        StatsServices statsServices,
        MvcBetContext context
    )
    {
        _findGamesService = findGames;
        _statsServices = statsServices;
        _context = context;
    }

    [HttpGet("Games/{id}")]
    public Game GetGames(int id)
    {
        var games = _findGamesService.GetGameList2().Result;
        var game = games.Where(x => id == x.FirstId || id == x.SecondId).First();
        // var oneTeam = new List<string>();
        // oneTeam.Add(game.TeamFirst);
        // oneTeam.Add(game.TeamSecond);

        return game;
    }

    [HttpGet("Games")]
    public List<Game> GetGames()
    {
        var games = _findGamesService.GetGameList2().Result;
        return games;
    }

    // [HttpGet("Games")]
    // public List<Games> GetGames()
    // {
    //     var games = _findGamesService.GetGameList().Result;
    //     return games;
    // }
    public async Task<ActionResult<GameWrapper>> GetGamesFromJson()
    {
        using FileStream openStream = System.IO.File.OpenRead(
            "C:/Users/troll/source/repos/Bet/wwwroot/game.json"
        );
        GameWrapper? gameInfo = await JsonSerializer.DeserializeAsync<GameWrapper>(openStream);
        return gameInfo;
    }

    public async Task<IActionResult> Index()
    {
        var httpClient = new HttpClient();
        var port = Request.Host.Port;
        var response = await httpClient.GetAsync($"https://localhost:{port}/Games");

        var gameList = await response.Content.ReadFromJsonAsync<List<Game>>();
        // var gameList = await response.Content.ReadFromJsonAsync<List<Games>>();
        return View(gameList);
    }

    public async Task<IActionResult> Privacy(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // using ConvertServices
        var gamelist = await GetGamesFromJson();
        Games? game = gamelist.Value?.Games.First(x => x.GameId == id);

        //using StatsServices
        string firstTeam = game.TeamFirstLogoSrc.ToUpper();
        string secondTeam = game.TeamSecondLogoSrc.ToUpper();
        var streak = await _statsServices.GetStreakAsync(firstTeam, secondTeam);

        List<Stats> stats = new();
        foreach (var item in streak)
        {
            stats.Add(
                new Stats
                {
                    wins = item[0],
                    loses = item[1],
                    streak = item[2],
                    lastTeen = item[3],
                    lastTeenProcent = item[4],
                    allWinRate = item[5],
                    lastFive = item[6],
                    lastFiveProcent = item[7]
                }
            );
        }

        var viewModel = new StatViewModel { Game = game, Stats = stats };

        return View(viewModel);
    }

    public class SelectedButtonsModel
    {
        public Dictionary<string, string> SelectedButtonIds { get; set; }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
