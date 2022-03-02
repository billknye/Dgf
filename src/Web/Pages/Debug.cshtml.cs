using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dgf.Framework;
using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dgf.Web.Pages
{
    public class DebugModel : PageModel
    {
        private readonly IEnumerable<IGame> games;
        private readonly IGameStateSerializer gameStateSerializer;

        public IGame Game { get; set; }
        public IGameState GameState { get; set; }
        public string JsonState { get; set; }

        public DebugModel(IEnumerable<IGame> games, IGameStateSerializer gameStateSerializer)
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

            JsonState = System.Text.Json.JsonSerializer.Serialize(GameState, Game.GameStateType, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
                WriteIndented = true
            });
            return Page();
        }
        public string GetUrl(IGameState state)
        {
            return Url.Page("Play", new { state = gameStateSerializer.Serialize(state) });
        }
    }
}