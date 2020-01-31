using LetPortal.Portal.Entities.SectionParts;
using System.Collections.Generic;

namespace LetPortal.Versions
{
    public class VersionUtils
    {
        public static void GenerateControlEvents(StandardComponent standardComponent)
        {
            foreach(var control in standardComponent.Controls)
            {
                switch(control.Type)
                {
                    case Portal.Entities.SectionParts.Controls.ControlType.Label:
                        control.PageControlEvents = new List<Portal.Entities.Components.Controls.PageControlEvent>();
                        break;
                    case Portal.Entities.SectionParts.Controls.ControlType.AutoComplete:
                        control.PageControlEvents = new List<Portal.Entities.Components.Controls.PageControlEvent>
                        {
                            new Portal.Entities.Components.Controls.PageControlEvent
                            {
                                EventName = control.Name + "_select",
                                EventActionType = Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
                                HttpServiceOptions = new Portal.Entities.Components.Controls.EventHttpServiceOptions
                                {
                                    BoundData = System.Array.Empty<string>(),
                                    HttpMethod = "Get",
                                    HttpServiceUrl = "",
                                    HttpSuccessCode = "200",
                                    JsonBody = "",
                                    OutputProjection = ""
                                },
                                TriggerEventOptions = new Portal.Entities.Components.Controls.TriggerEventOptions
                                {
                                    EventsList = System.Array.Empty<string>()
                                }
                            },
                            new Portal.Entities.Components.Controls.PageControlEvent
                            {
                                EventName = control.Name + "_change",
                                EventActionType = Portal.Entities.Components.Controls.EventActionType.TriggerEvent,
                                HttpServiceOptions = new Portal.Entities.Components.Controls.EventHttpServiceOptions
                                {
                                    BoundData = System.Array.Empty<string>(),
                                    HttpMethod = "Get",
                                    HttpServiceUrl = "",
                                    HttpSuccessCode = "200",
                                    JsonBody = "",
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
                                HttpServiceOptions = new Portal.Entities.Components.Controls.EventHttpServiceOptions
                                {
                                    BoundData = System.Array.Empty<string>(),
                                    HttpMethod = "Get",
                                    HttpServiceUrl = "",
                                    HttpSuccessCode = "200",
                                    JsonBody = "",
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
    }
}
