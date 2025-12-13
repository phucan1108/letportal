using LetPortal.Portal.Entities.Components.Controls;

namespace LetPortal.Portal.Entities.Pages
{
    public class PageEvent
    {
        public string EventName { get; set; }

        public EventActionType EventActionType { get; set; }

        public EventHttpServiceOptions HttpServiceOptions { get; set; }

        public TriggerEventOptions TriggerEventOptions { get; set; }
    }
}
