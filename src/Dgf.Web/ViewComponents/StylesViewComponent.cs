using Dgf.Framework;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dgf.Web.ViewComponents;

public class StylesViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var game = ViewData["Game"] as IGame;
        return View(game);
    }
}
