using Dgf.Framework;
using Dgf.Framework.States;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dgf.Web.ViewComponents;

public class ItemDisplayViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(IGame game, DisplayItem item, string href = null)
    {
        return View((game, item, href));
    }
}
