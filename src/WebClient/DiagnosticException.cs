using System;

namespace AV.Household.WebClient;

[Serializable]
internal class DiagnosticException : Exception
{
    public DiagnosticException(string id, string message) : base(message) => Id = id;
    public string Id { get; private set; }

    public string Category => "OPENAPIGEN";
}