namespace LetPortal.Portal.Models
{
    public class ExecuteDynamicResultModel
    {
        public dynamic Result { get; set; }

        public bool IsSuccess { get; set; }

        public string Error { get; set; }

        public static ExecuteDynamicResultModel IsFailed(string errorMessage)
        {
            return new ExecuteDynamicResultModel { IsSuccess = false, Error = errorMessage };
        }
    }

    public class StepExecutionResult
    {
        public StepExecutionType ExecutionType { get; set; }

        public dynamic Result { get; set; }

        public bool IsSuccess { get; set; }
        public string Error { get; set; }

        public static StepExecutionResult IsFailed(string errorMessage)
        {
            return new StepExecutionResult { IsSuccess = false, Error = errorMessage };
        }
    }
    public enum StepExecutionType
    {
        Query,
        Insert,
        Update,
        Delete,
        StoreProcedure,
        Multiple
    }
}
