using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using PersonalBlog.Tools;

namespace PersonalBlog.Tools.PageVersions
{
    public class PageVersion_0_0_1 : IPersonalBlogVersion
    {
        public string VersionNumber => "0.0.1";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<PageVersion>("5f108cac188c2043dca459b3");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {

            var homePageVersion = new PageVersion
            {
                Id = "5f108cac188c2043dca459b3",
                Name = "1",
                CreatedDate = DateTime.UtcNow,
                Creator = "admin",
                ModifiedDate = DateTime.UtcNow,
                PageId = "5f02cb57e63b2b3634d51375",
                Manifests = new List<PagePartManifest>
                {
                    new PagePartManifest
                    {
                        TemplateKey = "5fb12a67ba92b06448bcfeaf",
                        BindingType = LetPortal.CMS.Core.Abstractions.BindingType.Array,
                        ValuesList = new List<VersionValueList>
                        {
                            // Slide 1
                            new VersionValueList
                            {
                                Values = new List<VersionValue>
                                {
                                    new VersionValue
                                    {
                                        Key = "MediaSlide",
                                        EditorType = ManifestEditorType.MediaEditor,
                                        Value = "{\r\n  \"alt\":\".NET Core Feature\",\r\n  \"link\":\"data:image/gif;base64,R0lGODlhAQABAIAAAHd3dwAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==\",\r\n  \"mediaType\": 0\r\n}"
                                    },
                                    new VersionValue
                                    {
                                        Key = "Title",
                                        EditorType = ManifestEditorType.Textbox,
                                        Value = ".NET Core blog"
                                    },
                                    new VersionValue
                                    {
                                        Key = "Content",
                                        EditorType = ManifestEditorType.Textbox,
                                        Value = ".NET Core 3.1. LTS .NET Core is a free, cross-platform, open-source developer platform for building many different types."
                                    },
                                    new VersionValue
                                    {
                                        Key = "Link",
                                        EditorType = ManifestEditorType.LinkEditor,
                                        Value = "{\r\n  \"link\":\"blogs\",\r\n  \"display\":\"Read more\",\r\n  \"target\":\"_self\"\r\n}"
                                    }
                                }
                            },
                            // Slide 2
                            new VersionValueList
                            {
                                Values = new List<VersionValue>
                                {
                                    new VersionValue
                                    {
                                        Key = "MediaSlide",
                                        EditorType = ManifestEditorType.MediaEditor,
                                        Value = "{\r\n  \"alt\":\"Angular\",\r\n  \"link\":\"data:image/gif;base64,R0lGODlhAQABAIAAAHd3dwAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==\",\r\n  \"mediaType\": 0\r\n}"
                                    },
                                    new VersionValue
                                    {
                                        Key = "Title",
                                        EditorType = ManifestEditorType.Textbox,
                                        Value = "Angular blog"
                                    },
                                    new VersionValue
                                    {
                                        Key = "Content",
                                        EditorType = ManifestEditorType.Textbox,
                                        Value = "Angular is a platform for building mobile and desktop web applications. Join the community of millions of developers who build compelling user interfaces"
                                    },
                                    new VersionValue
                                    {
                                        Key = "Link",
                                        EditorType = ManifestEditorType.LinkEditor,
                                        Value = "{\r\n  \"link\":\"blogs\",\r\n  \"display\":\"Read more\",\r\n  \"target\":\"_self\"\r\n}"
                                    }
                                }
                            },
                            // Slide 3
                            new VersionValueList
                            {
                                Values = new List<VersionValue>
                                {
                                    new VersionValue
                                    {
                                        Key = "MediaSlide",
                                        EditorType = ManifestEditorType.MediaEditor,
                                        Value = "{\r\n  \"alt\":\"Let Portal\",\r\n  \"link\":\"data:image/gif;base64,R0lGODlhAQABAIAAAHd3dwAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==\",\r\n  \"mediaType\": 0\r\n}"
                                    },
                                    new VersionValue
                                    {
                                        Key = "Title",
                                        EditorType = ManifestEditorType.Textbox,
                                        Value = "Let Portal blog"
                                    },
                                    new VersionValue
                                    {
                                        Key = "Content",
                                        EditorType = ManifestEditorType.Textbox,
                                        Value = "LET Portal is new an open source web portal platform which helps to build quickly a application form, data list, chart, report, users management on 2020"
                                    },
                                    new VersionValue
                                    {
                                        Key = "Link",
                                        EditorType = ManifestEditorType.LinkEditor,
                                        Value = "{\r\n  \"link\":\"blogs\",\r\n  \"display\":\"Read more\",\r\n  \"target\":\"_self\"\r\n}"
                                    }
                                }
                            }
                        }
                    },
                    new PagePartManifest
                    {
                        TemplateKey = "5fb29a899d343d6714ac6fb6",
                        BindingType = LetPortal.CMS.Core.Abstractions.BindingType.Object,
                        ValuesList = new List<VersionValueList>
                        {
                            new VersionValueList
                            {
                                Values = new List<VersionValue>
                                {
                                    new VersionValue
                                    {
                                        Key = "Title",
                                        Value = ".NET Core Feature",
                                        EditorType = ManifestEditorType.Textbox
                                    },
                                    new VersionValue
                                    {
                                        Key = "Content",
                                        Value = ".NET Core 3.1. LTS .NET Core is a free, cross-platform, open-source developer platform for building many different types.",
                                        EditorType = ManifestEditorType.Textbox
                                    },
                                    new VersionValue
                                    {
                                        Key = "Media",
                                        Value = "{\r\n  \"alt\":\".NET Core Feature\",\r\n  \"link\":\"data:image/gif;base64,R0lGODlhAQABAIAAAHd3dwAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==\",\r\n  \"mediaType\": 0\r\n}",
                                        EditorType = ManifestEditorType.MediaEditor
                                    },
                                    new VersionValue
                                    {
                                        Key = "TargetLink",
                                        Value = "{\r\n  \"link\":\"blogs\",\r\n  \"display\":\"Read more\",\r\n  \"target\":\"_self\"\r\n}",
                                        EditorType = ManifestEditorType.LinkEditor
                                    }
                                }
                            }
                        }
                    },
                    new PagePartManifest
                    {
                        TemplateKey = "5fb2aeb66b98721d1c74fcc4",
                        BindingType = LetPortal.CMS.Core.Abstractions.BindingType.Object,
                        ValuesList = new List<VersionValueList>
                        {
                            new VersionValueList
                            {
                                Values = new List<VersionValue>
                                {
                                    new VersionValue
                                    {
                                        Key = "Title",
                                        Value = "Angular Feature",
                                        EditorType = ManifestEditorType.Textbox
                                    },
                                    new VersionValue
                                    {
                                        Key = "Content",
                                        Value = "Angular is a platform for building mobile and desktop web applications. Join the community of millions of developers who build compelling user interfaces",
                                        EditorType = ManifestEditorType.Textbox
                                    },
                                    new VersionValue
                                    {
                                        Key = "Media",
                                        Value = "{\r\n  \"alt\":\".NET Core Feature\",\r\n  \"link\":\"data:image/gif;base64,R0lGODlhAQABAIAAAHd3dwAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==\",\r\n  \"mediaType\": 0\r\n}",
                                        EditorType = ManifestEditorType.MediaEditor
                                    },
                                    new VersionValue
                                    {
                                        Key = "TargetLink",
                                        Value = "{\r\n  \"link\":\"blogs\",\r\n  \"display\":\"Read more\",\r\n  \"target\":\"_self\"\r\n}",
                                        EditorType = ManifestEditorType.LinkEditor
                                    }
                                }
                            }
                        }
                    }
                }
            };

            versionContext.InsertData(homePageVersion);
            return Task.CompletedTask;
        }
    }
}
