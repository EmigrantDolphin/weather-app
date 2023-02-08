namespace OneOfWrapper.Types;

public readonly struct BadRequest
{
    public BadRequest(string message)
    {
        Message = message;
    }
    
    public string Message { get; init; }
}
