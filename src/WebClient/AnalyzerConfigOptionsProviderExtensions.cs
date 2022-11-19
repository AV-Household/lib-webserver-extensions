using Microsoft.CodeAnalysis.Diagnostics;

namespace AV.Household.WebClient;

internal static class AnalyzerConfigOptionsProviderExtensions
{
    public static string GetRootDirectory(this AnalyzerConfigOptionsProvider analyzerConfig)
    {
        if (!analyzerConfig.GlobalOptions
                .TryGetValue("build_property.projectdir", out var projectDir)
            || string.IsNullOrEmpty(projectDir))
            throw new DiagnosticException("002", "Can't determine project root namespace.");

        return projectDir;
    }

    public static string GetRootNamespace(this AnalyzerConfigOptionsProvider analyzerConfig)
    {
        if (!analyzerConfig.GlobalOptions
                .TryGetValue("build_property.RootNamespace", out var rootNamespace)
            || string.IsNullOrEmpty(rootNamespace))
            throw new DiagnosticException("002", "Can't determine project root namespace.");

        return rootNamespace;
    }
}