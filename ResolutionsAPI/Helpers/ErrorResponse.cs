namespace ResolutionsApi.Helpers;

public static class ErrorResponse
{
    public static object BadRequest(string message, params string[] details)
    {
        return new
        {
            error = "BadRequest",
            message = message,
            details = details
        };
    }

    public static object NotFound(string message, params string[] details)
    {
        return new
        {
            error = "NotFound",
            message = message,
            details = details
        };
    }
}
