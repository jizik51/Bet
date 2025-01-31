using System.IO;
using System;
using Microsoft.AspNetCore.Mvc;

namespace Bet
{
    public class HomeController : Controller
    {
        private readonly FindGamesController _findGamesService;

        public HomeController(FindGamesController findGames)
        {
            _findGamesService = findGames;
        }

        public List<GameInfo> GetGames()
        {
            var games = _findGamesService.GetGameList2().Result;
            return games;
        }
    }
}
