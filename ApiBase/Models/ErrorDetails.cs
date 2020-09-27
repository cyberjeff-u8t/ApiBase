using System;

namespace Api.ErrorHandling
{
   
    public class NotFoundResponse
    {
        public string type { get; set; } = "type";

        public string title { get; set; } = "title";

        public string status { get; set; } = "status code";

        public string traceId { get; set; } = "traceId";

    }

    public class ErrorResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public ErrorResponse(Exception ex)
        {
            Type = ex.GetType().Name;
            Message = ex.Message;
            StackTrace = ex.ToString();
        }
    }
}