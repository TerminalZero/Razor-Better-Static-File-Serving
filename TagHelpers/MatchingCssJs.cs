using Microsoft.AspNetCore.Razor.TagHelpers;
using TZeroSolutions.AspNetCore.ApplicationBuilderExtensions;

namespace Helpers.TagHelpers
{
    /// <summary>
    /// This tag helper will add a script and link tag to the page referencing .cshtml.js and .cshtml.css files of the same name. <br/>
    /// Attributes "css-only" and "js-only" fetch either respectively, while omitting both will fetch both. <br/>
    /// Attribute "verify-file" will check if the file exists before adding the tag. <br/>
    /// </summary>
    [HtmlTargetElement("matching-css-js", TagStructure = TagStructure.WithoutEndTag)]
    public class MatchingCssJs : TagHelper
    {
        HttpContext? _httpContext;
        FilteredDirectoryService _directoryService;

        public bool CssOnly { get; set; }
        public bool JsOnly { get; set; }
        public bool verifyFiles { get; set; }

        public MatchingCssJs(IServiceProvider services, FilteredDirectoryService directoryService)
        {
            _httpContext = services.GetRequiredService<IHttpContextAccessor>().HttpContext;
            _directoryService = directoryService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (_httpContext == null) return;

            var jsFile = _httpContext.GetRouteData().Values.First().Value + ".cshtml.js";
            var cssFile = _httpContext.GetRouteData().Values.First().Value + ".cshtml.css";

            output.TagName = null;

            if (CssOnly)
            {
                if (verifyFiles && !_directoryService.FileExists(cssFile)) return;
                output.Content.SetHtmlContent($@"<link href=""{cssFile}"" rel=""stylesheet""/>");
                return;
            }
            if (JsOnly)
            {
                if (verifyFiles && !_directoryService.FileExists(jsFile)) return;
                output.Content.SetHtmlContent($@"<script src=""{jsFile}""></script>");
                return;
            }

            if (verifyFiles)
            {
                string content = "";
                if (_directoryService.FileExists(cssFile))
                {
                    content += $@"<link href=""{cssFile}"" rel=""stylesheet""/>";
                }
                if (_directoryService.FileExists(jsFile))
                {
                    content += $@"<script src=""{jsFile}""></script>";
                }
                output.Content.SetHtmlContent(content);
                return;
            }

            output.Content.SetHtmlContent($@"
                <link href=""{cssFile}"" rel=""stylesheet""/>
                <script src=""{jsFile}""></script>
            ");
        }
    }
}
