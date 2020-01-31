using System.Collections.Generic;

namespace LetPortal.Portal.Entities.Shared
{
    public class ButtonOptions
    {
        public ConfirmationOptions ConfirmationOptions { get; set; }

        public ActionCommandOptions ActionCommandOptions { get; set; }

        public RouteOptions RouteOptions { get; set; }
    }

    public class ConfirmationOptions
    {
        public bool IsEnable { get; set; }

        public string ConfirmationText { get; set; }
    }

    public class ActionOptions
    {
        public bool IsEnable { get; set; }

        public ActionCommandOptions CommandOptions { get; set; }
    }

    public class RouteOptions
    {
        public List<Route> Routes { get; set; }

        public bool IsEnable { get; set; }
    }

    public class Route
    {
        public RouteType RouteType { get; set; }

        public string TargetPageId { get; set; }

        public string TargetUrl { get; set; }

        public string PassDataPath { get; set; }

        public string Condition { get; set; }
    }

    public enum RouteType
    {
        ThroughPage,
        ThroughUrl
    }
}
