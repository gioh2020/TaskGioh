namespace TaskManagement.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public int ErrorStatusCode { get; private set; }

    private Result() { }

    public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };

    public static Result<T> Failure(string error, int statusCode = 400) => new Result<T>
    {
        IsSuccess = false,
        Error = error,
        ErrorStatusCode = statusCode
    };
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public int ErrorStatusCode { get; private set; }

    private Result() { }

    public static Result Success() => new Result { IsSuccess = true };

    public static Result Failure(string error, int statusCode = 400) => new Result
    {
        IsSuccess = false,
        Error = error,
        ErrorStatusCode = statusCode
    };
}
