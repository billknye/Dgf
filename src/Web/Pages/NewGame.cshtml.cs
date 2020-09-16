using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dgf.Framework;
using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dgf.Web.Pages
{
    public class NewGameModel : PageModel
    {
        private readonly IEnumerable<IGame> games;
        private readonly IGameStateSerializer gameStateSerializer;

        public IGame Game { get; set; }

        public string StateDescription { get; set; }

        [BindProperty]
        public string JsonState { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public NewGameModel(IEnumerable<IGame> games, IGameStateSerializer gameStateSerializer)
        {
            this.games = games;
            this.gameStateSerializer = gameStateSerializer;
        }

        public IActionResult OnGet(string slug)
        {
            Game = games.FirstOrDefault(n => n.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
            if (Game == null)
            {
                return NotFound();
            }
            ViewData["Game"] = Game;
            ViewData["Slug"] = slug;

            var startingState = Game.CreateStartingState();
            StateDescription = startingState.description;

            JsonState = System.Text.Json.JsonSerializer.Serialize(startingState.state, Game.GameStateType, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
                WriteIndented = true
            });

            return Page();
        }

        public IActionResult OnPost(string slug)
        {
            Game = games.FirstOrDefault(n => n.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
            if (Game == null)
            {
                return NotFound();
            }
            ViewData["Game"] = Game;
            ViewData["Slug"] = slug;

            var instance = System.Text.Json.JsonSerializer.Deserialize(JsonState, Game.GameStateType) as IGameState;

            var valid = Game.ValidateStartingState(instance, out var errors);
            Errors = errors;

            if (valid)
            {
                return RedirectToPage("Host", new { slug, state = gameStateSerializer.Serialize(instance) });
            }

            return Page();
        }
    }
}