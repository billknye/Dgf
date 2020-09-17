using Dgf.Framework;
using Markdig;
using Microsoft.AspNetCore.Html;

namespace Dgf.Web
{
    public static class CommonMark
    {
        static MarkdownPipeline pipeline;

        static CommonMark()
        {
            pipeline = new Markdig.MarkdownPipelineBuilder()
                .UseAdvancedExtensions().Build();  
        }

        public static IHtmlContent Encode(IGame game, string text)
        {
            var html = Markdig.Markdown.ToHtml(text, pipeline);
            //html = html.ReplaceSlug(game);
            return new HtmlString(html);
        }
    }
}
