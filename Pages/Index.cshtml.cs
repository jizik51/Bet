using Bet;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HomeController _gameService;
        public List<GameInfo> Games { get; set; }

        public IndexModel(HomeController gameService)
        {
            _gameService = gameService;
        }

        public void OnGet()
        {
            Games = _gameService.GetGames();
        }
    }
}
