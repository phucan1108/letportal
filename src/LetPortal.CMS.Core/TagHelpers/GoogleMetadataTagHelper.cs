using System;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Entities;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace LetPortal.CMS.Core.TagHelpers
{
    public class GoogleMetadataTagHelper : TagHelperComponent
    {
        private readonly ISiteRequestAccessor _siteRequest;

        public GoogleMetadataTagHelper(ISiteRequestAccessor siteRequest)
            : base()
        {
            _siteRequest = siteRequest;
        }

        public GoogleMetadata Metadata { get; set; }

        public override int Order => 1;

        public override Task ProcessAsync(TagHelperContext context,
                                         TagHelperOutput output)
        {
            Metadata = _siteRequest.Current.Page.GoogleMetadata;
            if (Metadata != null && string.Equals(context.TagName, "head",
                              StringComparison.OrdinalIgnoreCase))
            {
                var metadataBuilder = new StringBuilder();
                metadataBuilder.Append($"<title>{Metadata.Title}</title>");
                metadataBuilder.Append($"<meta name=\"description\" content=\"{Metadata.Description}\" />");                
                if (string.IsNullOrEmpty(Metadata.Robots))
                {
                    Metadata.Robots = "index, follow";
                }
                metadataBuilder.Append($"<meta name=\"robots\" content=\"{Metadata.Robots}\">");

                if (Metadata.IsAdultPage)
                {
                    metadataBuilder.Append("<meta name=\"rating\" content=\"adult\" />");
                }

                if (!Metadata.AllowGoogleTranslate)
                {
                    metadataBuilder.Append("<meta name=\"google\" content=\"notranslate\" />");
                }
                output.PostContent.AppendHtml(metadataBuilder.ToString());
            }
            return Task.CompletedTask;
        }
    }
}
