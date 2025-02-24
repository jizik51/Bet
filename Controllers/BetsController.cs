using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bet.Models;
using MvcBet.Data;

namespace Bet.Controllers
{
    [Route("Bets")]
    public class BetsController : Controller
    {
        private readonly MvcBetContext _context;

        public BetsController(MvcBetContext context)
        {
            _context = context;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            if (_context.Bets.Any()) // взят данные сюда и отправить в форму
            {
                var l = _context.Bets.ToList();
                return View(_context.Bets.ToList());
            }

            _context.Bets.Add(
                new BetViewModel
                {
                    id = 1,
                    FirstTeame = "Lal",
                    SecondTeam = "Orl",
                    whoWin = "П1",
                    gameResult = "null",
                    coefficent = "null"
                }
            );

            _context.SaveChanges();
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            BetViewModel games = _context.Bets.ToList().Find(x => x.id == id);
            _context.Bets.Remove(games);
            _context.SaveChanges();

            return View("Index", _context.Bets.ToList());
        }

        [HttpPost("UpdateData")]
        public async Task<IActionResult> UpdateData(BetViewModel betView)
        {
            Console.WriteLine($"{betView.gameResult}");

            BetViewModel game = _context.Bets.ToList().Find(x => x.id == betView.id);
            game.gameResult = betView.gameResult;
            game.coefficent = betView.coefficent;
            _context.SaveChanges();

            return View("Index", _context.Bets.ToList());
        }

        [HttpGet("Edit")]
        public IActionResult Edit(int id)
        {
            BetViewModel game = _context.Bets.ToList().Find(x => x.id == id);
            return View(game);
        }

        [HttpPost("AddToDb")]
        public async Task<IActionResult> AddToDb([FromBody] SelectedButtonsModel model)
        {
            if (model == null || model.SelectedButtonIds == null)
            {
                Console.WriteLine("Model is null");
                return BadRequest("Invalid data");
            }

            var listGame = new List<Game>(); // 4
            var dictionary = model.SelectedButtonIds.Keys.ToList();

            var httpClient = new HttpClient();
            var port = Request.Host.Port;

            for (var i = 0; i < model.SelectedButtonIds.Count; i++)
            {
                var response = await httpClient.GetAsync(
                    $"https://localhost:{port}/Games/{dictionary[i]}"
                );

                var game = await response.Content.ReadFromJsonAsync<Game>();
                listGame.Add(game);
            }
            var res = model.SelectedButtonIds.Values.ToList();

            for (var i = 0; i < listGame.Count; i++)
            {
                _context.Bets.Add(
                    new BetViewModel
                    {
                        FirstTeame = listGame[i].Home.Name,
                        SecondTeam = listGame[i].Away.Name,
                        whoWin = res[i],
                        gameResult = "null",
                        coefficent = "null"
                    }
                );
            }
            _context.SaveChanges();

            return View("Index", _context.Bets.ToList());
        }
    }
}

public class SelectedButtonsModel
{
    public Dictionary<string, string> SelectedButtonIds { get; set; }
}
