using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Entities.Components.Controls
{
    public class PageControlAsyncValidator
    {
        public string ValidatorName { get; set; }

        public bool IsActive { get; set; }

        public string ValidatorMessage { get; set; }

        public ControlAsyncValidatorOptions AsyncValidatorOptions { get; set; }
    }

    public class ControlAsyncValidatorOptions
    {
        public AsyncValidatorType ValidatorType { get; set; }

        public string EvaluatedExpression { get; set; }

        public SharedDatabaseOptions DatabaseOptions { get; set; }

        public HttpServiceOptions HttpServiceOptions { get; set; }
    }

    public enum AsyncValidatorType
    {
        DatabaseValidator,
        HttpValidator
    }
}
