using Dgf.Framework;
using Dgf.Framework.States.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dgf.Web.Pages;

public class SavesModel : GamePageModel
{
    public SavesModel(IEnumerable<IGame> games, IGameStateSerializer gameStateSerializer)
        : base(games, gameStateSerializer)
    {

    }
}
