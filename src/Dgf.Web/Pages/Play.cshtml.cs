using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dgf.Framework;
using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dgf.Web.Pages;

public class PlayModel : GamePageModel
{
    
    
    public PlayModel(IEnumerable<IGame> games, IGameStateSerializer gameStateSerializer)
        : base(games, gameStateSerializer)
    {

    }


}

public abstract class GamePageModel : PageModel
{
    private readonly IEnumerable<IGame> games;
    private readonly IGameStateSerializer gameStateSerializer;

    public IGame Game { get; set; }
    public IGameState GameState { get; set; }
    public string SerializedState { get; set; }

    public GameStateDescription GameStateDescription { get; set; }

    public GamePageModel(IEnumerable<IGame> games, IGameStateSerializer gameStateSerializer)
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

        SerializedState = state;
        GameState = gameStateSerializer.Deserialize(Game.GameStateType, state);
        GameStateDescription = Game.DescribeState(GameState);

        OnGetInternal();
        return Page();
    }

    protected virtual void OnGetInternal()
    {

    }

    public string GetUrl(IGameState state)
    {
        return Url.Page("Play", new { state = gameStateSerializer.Serialize(state) });
    }
}