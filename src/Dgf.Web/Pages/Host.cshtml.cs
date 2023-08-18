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

public class HostModel : GamePageModel
{

    public HostModel(IEnumerable<IGame> games, IGameStateSerializer gameStateSerializer)
        : base(games, gameStateSerializer)
    {

    }

    public string GetPlayUrl()
    {
        var url = Url.Page("Play", new { slug = Game.Slug, state = SerializedState });
        return url;
    }

    public string GetNewGameUrl()
    {
        var url = Url.Page("NewGame", new { slug = Game.Slug });
        return url;
    }
}
