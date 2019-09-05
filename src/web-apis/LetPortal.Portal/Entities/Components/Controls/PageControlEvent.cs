using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Entities.Components.Controls
{
    public class PageControlEvent
    {
        public string EventName { get; set; }

        public EventActionType EventActionType { get; set; }

        public EventHttpServiceOptions HttpServiceOptions { get; set; }

        public TriggerEventOptions TriggerEventOptions { get; set; }

    }

    public class TriggerEventOptions
    {
        public string[] EventsList { get; set; }
    }

    public class EventHttpServiceOptions : HttpServiceOptions
    {
        public string[] BoundData { get; set; }
    }

    public enum EventActionType
    {
        TriggerEvent,
        WebService
    }
}
