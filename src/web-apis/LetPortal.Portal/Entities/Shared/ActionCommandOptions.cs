namespace LetPortal.Portal.Entities.Shared
{
    public class ActionCommandOptions
    {
        public bool IsEnable { get; set; }

        public ActionType ActionType { get; set; } = ActionType.Redirect;

        public RedirectOptions RedirectOptions { get; set; }

        public HttpServiceOptions HttpServiceOptions { get; set; }

        public DatabaseExecutionChains DbExecutionChains { get; set; }

        public WorkflowOptions WorkflowOptions { get; set; }

        public NotificationOptions NotificationOptions { get; set; }
    }
}
