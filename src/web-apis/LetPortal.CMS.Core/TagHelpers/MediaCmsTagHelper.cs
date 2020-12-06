using System;
using LetPortal.CMS.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace LetPortal.CMS.Core.TagHelpers
{
    [HtmlTargetElement("media-cms")]
    public class MediaCmsTagHelper : TagHelper
    {
        public MediaModel Media { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Media.MediaType == MediaType.Image)
            {
                output.TagName = "img";
                output.Attributes.Add("src", Media.Link);
                output.Attributes.Add("alt", Media.Alt);
            }
            else if (Media.MediaType == MediaType.Video)
            {

            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
