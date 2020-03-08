namespace LetPortal.Portal.Models.Charts
{
    public class ExecutionChartResponseModel
    {
        public bool IsSuccess { get; set; }

        public dynamic Result { get; set; }

        public string Error { get; set; }

        public static ExecutionChartResponseModel IsFailed(string errorMessage)
        {
            return new ExecutionChartResponseModel { IsSuccess = false, Error = errorMessage };
        }
    }
}
