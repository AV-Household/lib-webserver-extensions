namespace AV.Household.WebServer.Extensions.Tasks;

/// <summary>
///     Class that encapsulates asynchronous result or exception occurs asynchronously
/// </summary>
/// <typeparam name="T">Type of function result</typeparam>
public class ResultOrException<T>
{
    /// <summary>
    ///     Creates from result value
    /// </summary>
    /// <param name="result">Result value</param>
    public ResultOrException(T result) => Result = result;

    /// <summary>
    ///     Creates from exception
    /// </summary>
    /// <param name="ex">Exception</param>
    public ResultOrException(Exception ex) => Exception = ex;

    /// <summary>
    ///     Flag of successful result
    /// </summary>
    public bool IsSuccess => Result is not null && Exception is null;

    /// <summary>
    ///     Result value or null
    /// </summary>
    public T? Result { get; }

    /// <summary>
    ///     Exception or null
    /// </summary>
    public Exception? Exception { get; }
}