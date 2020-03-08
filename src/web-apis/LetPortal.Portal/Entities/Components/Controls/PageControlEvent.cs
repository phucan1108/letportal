using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Entities.Components.Controls
{
    public class PageControlEvent
    {
        public string EventName { get; set; }

        public EventActionType EventActionType { get; set; }

        public EventHttpServiceOptions EventHttpServiceOptions { get; set; }

        public EventDatabaseOptions EventDatabaseOptions { get; set; }

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

    public class EventDatabaseOptions: DatabaseOptions
    {
        public string OutputProjection { get; set; }

        public string[] BoundData { get; set; }
    }

    public enum EventActionType
    {
        TriggerEvent,
        QueryDatabase,
        WebService
    }
}
