using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Versions;

namespace PersonalBlog.Tools.PageTemplates
{
    public class PageTemplate_0_0_1 : IPersonalBlogVersion
    {
        public string VersionNumber => "1.0.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<PageTemplate>("5f0aa3731558bf76447b18cb");
            versionContext.DeleteData<PageTemplate>("5f0aa3731558bf76447b18cc");
            versionContext.DeleteData<PageTemplate>("5f0aa3731558bf76447b18cd");
            versionContext.DeleteData<PageTemplate>("5f0aa3731558bf76447b18ce");
            versionContext.DeleteData<PageTemplate>("5f1ef6fce9ce36726865ebc0");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var homeTemplate = new PageTemplate
            {
                Id = "5f0aa3731558bf76447b18cb",
                Name = "Home Page",
                ThemeId = "5f02cb57e63b2b3634d51371",
                SiteId = Constants.MAIN_SITE_ID,
                Sections = new List<TemplateSection>
                {
                    new TemplateSection
                    {
                        Key = "5fb12a67ba92b06448bcfeaf",
                        Name = "Carousel",
                        BindingType = LetPortal.CMS.Core.Abstractions.BindingType.Array,
                        Hide = false,
                        ThemePartRef = "HomeCarousel",
                        Items = new List<TemplateDefaultItem>
                        {
                            new TemplateDefaultItem
                            {
                                Key = "MediaSlide",
                                EditorType = ManifestEditorType.MediaEditor
                            },
                            new TemplateDefaultItem
                            {
                                Key = "Title",
                                EditorType = ManifestEditorType.Textbox
                            },
                            new TemplateDefaultItem
                            {
                                Key = "Content",
                                EditorType = ManifestEditorType.Textbox
                            },
                            new TemplateDefaultItem
                            {
                                Key = "Link",
                                EditorType = ManifestEditorType.LinkEditor
                            }
                        }
                    },
                    new TemplateSection
                    {
                        Key = "5fb29a899d343d6714ac6fb6",
                        Name = ".NET Core Feature",
                        BindingType = LetPortal.CMS.Core.Abstractions.BindingType.Object,
                        ThemePartRef = "LeftFeature",
                        Items = new List<TemplateDefaultItem>
                        {
                            new TemplateDefaultItem
                            {
                                Key = "Title",
                                Value = "",
                                EditorType = ManifestEditorType.Textbox
                            },
                            new TemplateDefaultItem
                            {
                                Key = "Content",
                                Value = "",
                                EditorType = ManifestEditorType.Textbox
                            },
                            new TemplateDefaultItem
                            {
                                Key = "Media",
                                Value = "",
                                EditorType = ManifestEditorType.MediaEditor
                            },
                            new TemplateDefaultItem
                            {
                                Key = "TargetLink",
                                Value = "",
                                EditorType = ManifestEditorType.LinkEditor
                            }
                        }
                    },
                    new TemplateSection
                    {
                        Key = "5fb2aeb66b98721d1c74fcc4",
                        Name = "Angular Feature",
                        BindingType = LetPortal.CMS.Core.Abstractions.BindingType.Object,
                        ThemePartRef = "RightFeature",
                        Items = new List<TemplateDefaultItem>
                        {
                            new TemplateDefaultItem
                            {
                                Key = "Title",
                                Value = "",
                                EditorType = ManifestEditorType.Textbox
                            },
                            new TemplateDefaultItem
                            {
                                Key = "Content",
                                Value = "",
                                EditorType = ManifestEditorType.Textbox
                            },
                            new TemplateDefaultItem
                            {
                                Key = "Media",
                                Value = "",
                                EditorType = ManifestEditorType.MediaEditor
                            },
                            new TemplateDefaultItem
                            {
                                Key = "TargetLink",
                                Value = "",
                                EditorType = ManifestEditorType.LinkEditor
                            }
                        }
                    }
                }
            };

            var blogsTemplate = new PageTemplate
            {
                Id = "5f0aa3731558bf76447b18cc",
                Name = "Blogs",
                ThemeId = "5f02cb57e63b2b3634d51371",
                SiteId = Constants.MAIN_SITE_ID,
                Sections = new List<TemplateSection>
                {
                    new TemplateSection
                    {
                        Key = "5fb398bd8dbe2c62e0a674cd",
                        Name = "Blog Categories Header",
                        ThemePartRef = "BlogCategoriesHeader",
                        BindingType = LetPortal.CMS.Core.Abstractions.BindingType.None
                    },
                    new TemplateSection
                    {
                        Key = "5fb36abbf40cc36c8c865a42",
                        Name = "Blog Categories",
                        ThemePartRef = "BlogCategories",
                        BindingType = LetPortal.CMS.Core.Abstractions.BindingType.None
                    }
                }
            };

            var blogCateTemplate = new PageTemplate
            {
                Id = "5f0aa3731558bf76447b18cd",
                Name = "Blog Category",
                ThemeId = "5f02cb57e63b2b3634d51371",
                SiteId = Constants.MAIN_SITE_ID,
                Sections = new List<TemplateSection>
                {
                    new TemplateSection
                    {
                        Key = "5fb398bd8dbe2c62e0a674cf",
                        Name = "Blog Category",
                        ThemePartRef = "BlogCategory",
                        BindingType = LetPortal.CMS.Core.Abstractions.BindingType.None
                    }
                }
            };

            var blogPostTemplate = new PageTemplate
            {
                Id = "5f0aa3731558bf76447b18ce",
                Name = "Blog Post",
                ThemeId = "5f02cb57e63b2b3634d51371",
                SiteId = Constants.MAIN_SITE_ID,
                Sections = new List<TemplateSection>
                {
                   new TemplateSection
                   {
                       Key = "5fb398bd8dbe2c62e0a674d8",
                       Name = "Blog Post",
                       ThemePartRef = "BlogPost",
                       BindingType = LetPortal.CMS.Core.Abstractions.BindingType.None
                   }
                }
            };

            var blogTagsTemplate = new PageTemplate
            {
                Id = "5f1ef6fce9ce36726865ebc0",
                Name = "Blog Tags",
                ThemeId = "5f02cb57e63b2b3634d51371",
                SiteId = Constants.MAIN_SITE_ID,
                Sections = new List<TemplateSection>
                {
                    new TemplateSection
                    {
                        Name = "Blog Tags",
                        ThemePartRef = "BlogTags"
                    }
                }
            };

            versionContext.InsertData(homeTemplate);
            versionContext.InsertData(blogsTemplate);
            versionContext.InsertData(blogCateTemplate);
            versionContext.InsertData(blogPostTemplate);
            versionContext.InsertData(blogTagsTemplate);
            return Task.CompletedTask;
        }
    }
}
