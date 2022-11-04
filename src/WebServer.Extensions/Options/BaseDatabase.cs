namespace AV.Household.WebServer.Extensions.Options;

/// <summary>
///     Basic options for database connection
/// </summary>
public abstract class BaseDatabase
{
    /// <summary>
    ///     DBMS connection string
    /// </summary>
    public string ConnectionString { get; set; } = "Unset connection string";

    /// <summary>
    ///     Database name
    /// </summary>
    public string Database { get; set; } = "Unknown database";
}