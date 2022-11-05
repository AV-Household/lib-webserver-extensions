using System.Runtime.CompilerServices;

namespace AV.Household.WebServer.Testing.Tasks;

/// <summary>
///     Wrapper class representing shorter syntax of Lazy&lt;Task&lt;T&gt;&gt;"/&gt;.
///     Useful when declaring a lazy async factory of value T.
/// </summary>
/// <typeparam name="T">Value type</typeparam>
public class AsyncLazy<T>
{
    private readonly Lazy<Task<T>> _inner;

    /// <summary>
    ///     Constructs from async factory function
    /// </summary>
    /// <param name="valueFactory"></param>
    public AsyncLazy(Func<Task<T>> valueFactory) => _inner = new Lazy<Task<T>>(valueFactory);

    /// <summary>
    ///     Constructs from lazy type initializer
    /// </summary>
    /// <param name="inner">Lazy initializer</param>
    public AsyncLazy(Lazy<Task<T>> inner) => _inner = inner;

    /// <summary>
    ///     Implement GetAwaiter tot use async/await syntax
    /// </summary>
    /// <returns>Task awaiter object</returns>
    public TaskAwaiter<T> GetAwaiter() => _inner.Value.GetAwaiter();

    /// <summary>
    ///     Implicit operator for cast to synchronized lazy object
    /// </summary>
    /// <param name="outer"></param>
    /// <returns></returns>
    public static implicit operator Lazy<Task<T>>(AsyncLazy<T> outer) => outer._inner;
}