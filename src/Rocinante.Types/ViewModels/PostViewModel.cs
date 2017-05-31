using Microsoft.AspNetCore.Html;

namespace Rocinante.Types.ViewModels
{
    public class PostViewModel
    {
        public Site Site { get; set; }
        public Post Post { get; set; }
        public HtmlString RenderedPost { get; set; }
    }
}