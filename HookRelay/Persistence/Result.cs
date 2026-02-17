namespace HookRelay.Persistence;

public class Result<T>
{
    public T Value { get; private set; }
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    
    public string ErrorMessage { get; private set; }

    public Result(T value)
    {
        Value = value;
        IsSuccess = true;
    }

    public Result(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }
    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(errorMessage);
    }


}