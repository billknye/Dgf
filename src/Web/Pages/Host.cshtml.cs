using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dgf.Framework;
using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dgf.Web.Pages
{
    public class HostModel : PageModel
    {
        private readonly IEnumerable<IGame> games;
        private readonly IGameStateSerializer gameStateSerializer;

        public IGame Game { get; set; }
        public IGameState GameState { get; set; }

        public GameStateDescription GameStateDescription { get; set; }

        public HostModel(IEnumerable<IGame> games, IGameStateSerializer gameStateSerializer)
        {
            this.games = games;
            this.gameStateSerializer = gameStateSerializer;
        }

        public IActionResult OnGet(string slug, string state)
        {
            Game = games.FirstOrDefault(n => n.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
            if (Game == null)
            {
                return NotFound();
            }

            ViewData["Game"] = Game;
            ViewData["Slug"] = slug;

            GameState = gameStateSerializer.Deserialize(Game.GameStateType, state);
            GameStateDescription = Game.DescribeState(GameState);

            return Page();
        }
        public string GetUrl(IGameState state)
        {
            var url = Url.Page("Play", new { slug = Game.Slug, state = gameStateSerializer.Serialize(state) });

            return url;
        }
    }
}
