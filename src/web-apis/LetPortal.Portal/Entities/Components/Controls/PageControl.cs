using LetPortal.Portal.Entities.Components.Controls;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;
using System.Collections.Generic;

namespace LetPortal.Portal.Entities.SectionParts.Controls
{
    public class PageControl
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DatasourceOptions DatasourceOptions { get; set; } = new DatasourceOptions
        {
            Type = DatasourceControlType.StaticResource,
            DatabaseOptions = new DatabaseOptions
            {
                DatabaseConnectionId = "",
                EntityName = "",
                Query = ""
            },
            DatasourceStaticOptions = new DatasourceStaticOptions
            {
                JsonResource = ""
            },
            HttpServiceOptions = new HttpServiceOptions
            {
                HttpMethod = "Get",
                HttpServiceUrl = "",
                HttpSuccessCode = "200",
                JsonBody = "",
                OutputProjection = ""
            }
        };

        public bool IsActive { get; set; }

        public int Order { get; set; }

        public ControlType Type { get; set; }

        public List<PageControlValidator> Validators { get; set; } = new List<PageControlValidator>();

        public List<PageControlAsyncValidator> AsyncValidators { get; set; } = new List<PageControlAsyncValidator>();

        public List<ShellOption> Options { get; set; } = new List<ShellOption>();

        public List<PageControlEvent> PageControlEvents { get; set; } = new List<PageControlEvent>();
    }

    public enum ControlType
    {
        LineBreaker,
        Label,
        Textbox,
        Textarea,
        Number,
        Email,
        Password,
        DateTime,
        Checkbox,
        Slide,
        Radio,
        Select,
        AutoComplete,
        RichTextEditor,
        Uploader,
        MultiUploader,
        IconPicker,    
        MarkdownEditor
    }
}
