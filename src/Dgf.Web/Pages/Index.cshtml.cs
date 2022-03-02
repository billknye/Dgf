using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dgf.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Dgf.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IEnumerable<IGame> games;

    public IEnumerable<IGame> Games => games;

    public IndexModel(ILogger<IndexModel> logger, IEnumerable<IGame> games)
    {
        _logger = logger;
        this.games = games;
    }

    public void OnGet()
    {

    }
}
