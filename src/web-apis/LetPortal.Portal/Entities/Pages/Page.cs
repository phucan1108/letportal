using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Core.Security;
using LetPortal.Portal.Entities.Localizations;

namespace LetPortal.Portal.Entities.Pages
{
    [EntityCollection(Name = "pages")]
    [Table("pages")]
    public class Page : BackupableEntity, ICodeGenerable
    {
        [StringLength(250)]
        public string UrlPath { get; set; }

        [StringLength(50)]
        public string AppId { get; set; }

        public List<ShellOption> ShellOptions { get; set; }

        public List<PortalClaim> Claims { get; set; }

        public PageBuilder Builder { get; set; }

        public List<PageDatasource> PageDatasources { get; set; }

        public List<PageEvent> Events { get; set; }

        public List<PageButton> Commands { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult
            {
                DeletingCode = $"versionContext.DeleteData<LetPortal.Portal.Entities.Pages.Page>(\"{Id}\");"
            };
            var stringBuilder = new StringBuilder();
            varName ??= Name.Replace("-", "", System.StringComparison.OrdinalIgnoreCase) + "Page";
            _ = stringBuilder.AppendLine($"var {varName} = new LetPortal.Portal.Entities.Pages.Page");
            _ = stringBuilder.AppendLine($"{{");
            _ = stringBuilder.AppendLine($"    Id = \"{Id}\",");
            _ = stringBuilder.AppendLine($"    Name = \"{Name}\",");
            _ = stringBuilder.AppendLine($"    DisplayName = \"{DisplayName}\",");
            _ = stringBuilder.AppendLine($"    AppId = \"{AppId}\",");
            _ = stringBuilder.AppendLine($"    UrlPath = \"{UrlPath}\",");
            if (ShellOptions != null && ShellOptions.Any())               {
                _ = stringBuilder.AppendLine($"    ShellOptions = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>");
                _ = stringBuilder.AppendLine($"    {{");
                foreach (var option in ShellOptions)
                {
                    _ = stringBuilder.AppendLine($"        new LetPortal.Portal.Entities.Pages.ShellOption");
                    _ = stringBuilder.AppendLine($"        {{");
                    _ = stringBuilder.AppendLine($"            Key = \"{option.Key}\",");
                    _ = stringBuilder.AppendLine($"            Value = \"{option.Value}\",");
                    _ = stringBuilder.AppendLine($"            Description = \"{option.Description}\"");
                    _ = stringBuilder.AppendLine($"        }},");
                }
                _ = stringBuilder.AppendLine($"    }},");
            }
            if(Claims != null && Claims.Any())
            {
                _ = stringBuilder.AppendLine($"    Claims = new System.Collections.Generic.List<LetPortal.Core.Security.PortalClaim>");
                _ = stringBuilder.AppendLine($"    {{");
                foreach (var claim in Claims)
                {
                    _ = stringBuilder.AppendLine($"        new LetPortal.Core.Security.PortalClaim");
                    _ = stringBuilder.AppendLine($"        {{");
                    _ = stringBuilder.AppendLine($"            Name = \"{claim.Name}\",");
                    _ = stringBuilder.AppendLine($"            DisplayName = \"{claim.DisplayName}\",");
                    var claimType = "LetPortal.Core.Security.ClaimValueType." + Enum.GetName(typeof(ClaimValueType), claim.ClaimValueType);
                    _ = stringBuilder.AppendLine($"            ClaimValueType = {claimType}");
                    _ = stringBuilder.AppendLine($"        }},");
                }
                _ = stringBuilder.AppendLine($"    }},");
            }
            if(Builder != null)
            {
                _ = stringBuilder.AppendLine(Builder.GenerateCode().InsertingCode);
            }

            if(PageDatasources != null && PageDatasources.Any())
            {
                _ = stringBuilder.AppendLine($"    PageDatasources = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageDatasource>");
                _ = stringBuilder.AppendLine($"    {{");
                foreach(var pageDatasource in PageDatasources)
                {
                    _ = stringBuilder.AppendLine($"        new LetPortal.Portal.Entities.Pages.PageDatasource");
                    _ = stringBuilder.AppendLine($"        {{");
                    _ = stringBuilder.AppendLine($"           Id = \"{pageDatasource.Id}\",");
                    _ = stringBuilder.AppendLine($"           Name = \"{pageDatasource.Name}\",");
                    _ = stringBuilder.AppendLine($"           TriggerCondition = \"{pageDatasource.TriggerCondition}\",");
                    _ = stringBuilder.AppendLine($"           IsActive = {pageDatasource.IsActive.ToString().ToLower()},");
                    _ = stringBuilder.AppendLine(pageDatasource.Options.GenerateCode("Options", space = 2).InsertingCode);
                    _ = stringBuilder.AppendLine($"        }},");
                }
                _ = stringBuilder.AppendLine($"    }},");
            }

            if(Commands != null && Commands.Any())
            {
                _ = stringBuilder.AppendLine($"    Commands = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageButton>");
                _ = stringBuilder.AppendLine($"    {{");
                foreach(var command in Commands)
                {
                    _ = stringBuilder.AppendLine($"        new LetPortal.Portal.Entities.Pages.PageButton");
                    _ = stringBuilder.AppendLine($"        {{");
                    _ = stringBuilder.AppendLine($"            Id = \"{command.Id}\",");
                    _ = stringBuilder.AppendLine($"            Name = \"{command.Name}\",");
                    _ = stringBuilder.AppendLine($"            Icon = \"{command.Icon}\",");
                    _ = stringBuilder.AppendLine($"            Color = \"{command.Color}\",");
                    _ = stringBuilder.AppendLine($"            AllowHidden = \"{command.AllowHidden}\",");
                    _ = stringBuilder.AppendLine($"            PlaceSectionId = \"{command.PlaceSectionId}\",");
                    _ = stringBuilder.AppendLine($"            IsRequiredValidation = {command.IsRequiredValidation.ToString().ToLower()},");
                    _ = stringBuilder.AppendLine(command.ButtonOptions.GenerateCode(space: 3).InsertingCode);                    
                    _ = stringBuilder.AppendLine($"        }},");
                }
                _ = stringBuilder.AppendLine($"    }},");
            }
            _ = stringBuilder.AppendLine($"}};");
            _ = stringBuilder.AppendLine($"versionContext.InsertData({varName});");
            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }
    }

    #region Page Info

    public class ShellOption
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }

    #endregion

    #region Page Builder
    public class PageBuilder : ICodeGenerable
    {
        public List<PageSection> Sections { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();

            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"    Builder = new LetPortal.Portal.Entities.Pages.PageBuilder");
            _ = stringBuilder.AppendLine($"    {{");
            if(Sections != null && Sections.Any())
            {
                _ = stringBuilder.AppendLine($"        Sections = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.PageSection>");
                _ = stringBuilder.AppendLine($"        {{");
                foreach(var section in Sections)
                {
                    _ = stringBuilder.AppendLine($"            new LetPortal.Portal.Entities.Pages.PageSection");
                    _ = stringBuilder.AppendLine($"            {{");
                    _ = stringBuilder.AppendLine($"                Id = \"{section.Id}\",");
                    _ = stringBuilder.AppendLine($"                ComponentId = \"{section.ComponentId}\",");
                    _ = stringBuilder.AppendLine($"                Name = \"{section.Name}\",");
                    _ = stringBuilder.AppendLine($"                DisplayName = \"{section.DisplayName}\",");
                    var constructionType = "LetPortal.Portal.Entities.Pages.SectionContructionType." + Enum.GetName(typeof(SectionContructionType), section.ConstructionType);
                    _ = stringBuilder.AppendLine($"                ConstructionType = {constructionType},");
                    _ = stringBuilder.AppendLine($"                Hidden = \"{section.Hidden}\",");
                    if (section.SectionDatasource != null)
                    {
                        _ = stringBuilder.AppendLine($"                SectionDatasource = new LetPortal.Portal.Entities.Pages.SectionDatasource");
                        _ = stringBuilder.AppendLine($"                {{");
                        _ = stringBuilder.AppendLine($"                    DatasourceBindName = \"{section.SectionDatasource.DatasourceBindName}\",");
                        _ = stringBuilder.AppendLine($"                    DataStoreName = \"{section.SectionDatasource.DataStoreName}\"");
                        _ = stringBuilder.AppendLine($"                }},");
                    }
                    if(section != Sections.Last())
                    {
                        _ = stringBuilder.AppendLine($"            }},");
                    }
                    else
                    {
                        _ = stringBuilder.AppendLine($"            }}");
                    }
                    
                }
                _ = stringBuilder.AppendLine($"        }}");
            }
            _ = stringBuilder.AppendLine($"    }},");

            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }
    }
    #endregion
}
