using OneOf;

namespace OneOfWrapper;

public static class OneOfExtensions
{
    public static TSuccess AsSuccess<TSuccess, T>(this OneOf<TSuccess, T> oneOf) => oneOf.AsT0;
    public static TSuccess AsSuccess<TSuccess, T0, T1>(this OneOf<TSuccess, T0, T1> oneOf) => oneOf.AsT0;
    public static bool IsSuccess<TSuccess, T>(this OneOf<TSuccess, T> oneOf) => oneOf.IsT0;
    public static bool IsSuccess<TSuccess, T0, T1>(this OneOf<TSuccess, T0, T1> oneOf) => oneOf.IsT0;
    
    public static TError AsError<TSuccess, TError>(this OneOf<TSuccess, TError> oneOf) => oneOf.AsT1;
    public static bool IsError<TSuccess, TError>(this OneOf<TSuccess, TError> oneOf) => oneOf.IsT1;
}
