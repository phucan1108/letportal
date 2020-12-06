using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.CMS.Features.Blogs.Entities
{
    [EntityCollection(Name = "posts")]
    public class Post : Entity
    {
        /// <summary>
        /// Very important field, help to look up via URL
        /// This field will be generated automatically
        /// However, each lang had different way to generate correctly Unicode Url
        /// So creator must handle this field manually
        /// </summary>
        public string UrlPath { get; set; }

        public string ScreenshotUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Author { get; set; }

        /// <summary>
        /// Contains TagId
        /// </summary>
        public IList<string> Tags { get; set; }

        /// <summary>
        /// Allow user can read comment
        /// </summary>
        public bool ShowComment { get; set; }

        /// <summary>
        /// Allow user can write comment
        /// </summary>
        public bool AllowUserComment { get; set; }

        /// <summary>
        /// Prevent over submitting, maximum comments/post/user/day. Default: 5
        /// </summary>
        public int MaximumCommentPerDay { get; set; } = 5;

        /// <summary>
        /// This field can be calculated by create/edit. Default: 1m = 300 words
        /// </summary>
        public int ReadDuration { get; set; }

        public DateTime CreatedDate { get; set; }

        public string BlogId { get; set; }
    } 
}
