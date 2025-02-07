using System.IO;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json;
using Bet.Models;
using System.Diagnostics;

// using RazorPagesApp.Pages;
namespace Bet.Controllers;

// [ApiController]
// [Route("[controller]")]
public class HomeController : Controller
{
    private readonly FindGamesController _findGamesService;

    public HomeController(FindGamesController findGames)
    {
        _findGamesService = findGames;
    }

    [HttpGet("Games")]
    public List<Games> GetGames()
    {
        var games = _findGamesService.GetGameList().Result;
        return games;
    }

    // [HttpGet]
    // public async Task<ActionResult<GameInfo>> GetGames1()
    // {
    //     // var rr = GetGames();
    //     var json = System.IO.File.ReadAllText(
    //         "C:/Users/troll/source/repos/Bet/wwwroot/game.json"
    //     );

    //     var gameInfo = JsonSerializer.Deserialize<GameInfo>(json);
    //     return gameInfo;
    // }

    public async Task<IActionResult> Index()
    {
        var httpClient = new HttpClient();
        var port = Request.Host.Port;
        var response = await httpClient.GetAsync($"https://localhost:{port}/Games");
        var gameList = await response.Content.ReadFromJsonAsync<List<Games>>();

        return View(gameList);
    }

    public IActionResult Privacy()
    {
        // Call FromJsonToGamesList
        // View(ListGames)
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
