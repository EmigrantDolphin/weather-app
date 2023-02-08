namespace OneOfWrapper.Types;

public readonly struct Success { }

public readonly struct Success<T>
{
    public Success(T value)
    {
        Value = value;
    }
    
    public T Value { get; }
}
