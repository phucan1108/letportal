using System.Collections.Generic;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Versions
{
    public class VersionUtils
    {
        public static void GenerateControlEvents(StandardComponent standardComponent)
        {
            foreach (var control in standardComponent.Controls)
            {
                switch (control.Type)
                {
                    case Portal.Entities.SectionParts.Controls.ControlType.Label:
                        control.PageControlEvents = new List<Portal.Entities.Components.Controls.PageControlEvent>();
                        break;
                    case Portal.Entities.SectionParts.Controls.ControlType.AutoComplete:
                        control.PageControlEvents = new List<Portal.Entities.Components.Controls.PageControlEvent>
                        {
                            new Portal.Entities.Components.Controls.PageControlEvent
                            {
                                EventName = control.Name + "_change",
                                EventActionType = Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
                                EventHttpServiceOptions = new Portal.Entities.Components.Controls.EventHttpServiceOptions
                                {
                                    BoundData = System.Array.Empty<string>(),
                                    HttpMethod = "Get",
                                    HttpServiceUrl = "",
                                    HttpSuccessCode = "200",
                                    JsonBody = "",
                                    OutputProjection = ""
                                },
                                EventDatabaseOptions = new Portal.Entities.Components.Controls.EventDatabaseOptions
                                {
                                    BoundData = System.Array.Empty<string>(),
                                    DatabaseConnectionId = "",
                                    EntityName = "",
                                    Query = "",
                                    OutputProjection = ""
                                },
                                TriggerEventOptions = new Portal.Entities.Components.Controls.TriggerEventOptions
                                {
                                    EventsList = System.Array.Empty<string>()
                                }
                            }
                        };
                        break;
                    default:
                        control.PageControlEvents = new List<Portal.Entities.Components.Controls.PageControlEvent>
                        {
                            new Portal.Entities.Components.Controls.PageControlEvent
                            {
                                EventName = control.Name + "_change",
                                EventActionType = Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
                                EventHttpServiceOptions = new Portal.Entities.Components.Controls.EventHttpServiceOptions
                                {
                                    BoundData = System.Array.Empty<string>(),
                                    HttpMethod = "Get",
                                    HttpServiceUrl = "",
                                    HttpSuccessCode = "200",
                                    JsonBody = "",
                                    OutputProjection = ""
                                },
                                EventDatabaseOptions = new Portal.Entities.Components.Controls.EventDatabaseOptions
                                {
                                    BoundData = System.Array.Empty<string>(),
                                    DatabaseConnectionId = "",
                                    EntityName = "",
                                    Query = "",
                                    OutputProjection = ""
                                },  
                                TriggerEventOptions = new Portal.Entities.Components.Controls.TriggerEventOptions
                                {
                                    EventsList = System.Array.Empty<string>()
                                }
                            }
                        };
                        break;
                }
            }
        }

        public static void GenerateStandardOptions(StandardComponent standardComponent)
        {
            standardComponent.Options = new List<Portal.Entities.Pages.ShellOption>();

            standardComponent.Options.Add(new Portal.Entities.Pages.ShellOption
            {
                Key = "identityfield",
                Value = "id"
            });

            standardComponent.Options.Add(new Portal.Entities.Pages.ShellOption
            {
                Key = "namefield",
                Value = "name"
            });

            standardComponent.Options.Add(new Portal.Entities.Pages.ShellOption
            {
                Key = "allowadjustment",
                Value = "false"
            });

            standardComponent.Options.Add(new Portal.Entities.Pages.ShellOption
            {
                Key = "allowupdateparts",
                Value = "false"
            });
        }
    }
}
