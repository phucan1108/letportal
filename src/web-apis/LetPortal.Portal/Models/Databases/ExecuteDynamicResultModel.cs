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
}
