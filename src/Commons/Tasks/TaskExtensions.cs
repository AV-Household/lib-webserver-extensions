namespace AV.Household.Commons.Tasks;

/// <summary>
///     Extension methods for Task class
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    ///     Get task result and ignores exception
    /// </summary>
    /// <param name="task">Asynchronous task</param>
    /// <typeparam name="T">Type of task result</typeparam>
    /// <returns>Result or exception</returns>
    public static async Task<ResultOrException<T>> GetResultOrException<T>(this Task<T> task)
    {
        try
        {
            var result = await task;
            return new ResultOrException<T>(result);
        }
        catch (Exception ex)
        {
            return new ResultOrException<T>(ex);
        }
    }
}