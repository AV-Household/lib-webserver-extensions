using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;

namespace AV.Household.WebClient;

/// <summary>
///     Incremental source generator
///     for code generation from Open API JSON format
/// </summary>
[Generator(LanguageNames.CSharp)]
public class OpenApiClientGenerator : IIncrementalGenerator
{
    /// <inheritdoc cref="IIncrementalGenerator" />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var openApiFiles = context.AdditionalTextsProvider
            .Where(static text => text.Path.EndsWith(".openapi", StringComparison.InvariantCultureIgnoreCase))
            .Collect();

        context.RegisterSourceOutput(
            context.CompilationProvider
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Combine(openApiFiles),
            static (context, tuple) => Generate(tuple.Left.Right, tuple.Right, context));
    }

    private static void Generate(AnalyzerConfigOptionsProvider analyzerConfig,
        ImmutableArray<AdditionalText> openApiFiles, SourceProductionContext context)
    {
        string projectDir, rootNamespace;

        try
        {
            projectDir = analyzerConfig.GetRootDirectory();
            rootNamespace = analyzerConfig.GetRootNamespace();

            foreach (var file in openApiFiles)
                Task.Run(() =>
                {
                    var (@namespace, @class) = GetFullNameByNamingConvention(file.Path);
                    var source = GenerateClient(file, @namespace, @class);
                    context.AddSource($"{Path.GetFileName(file.Path)}.cs", SourceText.From(source, Encoding.UTF8));
                }).Wait();
        }
        catch (DiagnosticException ex)
        {
            context.ReportDiagnostic(Diagnostic.Create(ex.Id, ex.Category, ex.Message,
                DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0));
        }
        catch (Exception ex)
        {
            context.ReportDiagnostic(Diagnostic.Create("001", "UNEXPECT",
                ex.Message + typeof(OpenApiClientGenerator).Assembly.Location,
                DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0));
        }

        (string @namespace, string @class) GetFullNameByNamingConvention(string file)
        {
            var fileDir = Path.GetDirectoryName(file);
            if (fileDir != null && !fileDir.StartsWith(projectDir))
                throw new DiagnosticException("003", "File not in project root directory subdirectory.");

            var relativePath = fileDir!.Remove(0, projectDir.Length);
            var namespaceParts = new[] {rootNamespace}
                .Union(relativePath.Split('\\', '/')
                    .Where(x => !string.IsNullOrEmpty(x.Trim())));

            var @namespace = namespaceParts.Aggregate((x, y) => $"{x}.{y}");
            var @class = Path.GetFileNameWithoutExtension(file);

            if (string.IsNullOrEmpty(@namespace))
                throw new DiagnosticException("005", "Can't determine target namespace.");

            if (string.IsNullOrEmpty(@class))
                throw new DiagnosticException("006", "Can't determine target class name.");

            return (@namespace, @class);
        }
    }

    private static string GenerateClient(AdditionalText file, string @namespace, string @class)
    {
        try
        {
            var openApi = OpenApiDocument.FromFileAsync(file.Path).Result;

            var generator = new CSharpClientGenerator(openApi, new CSharpClientGeneratorSettings
            {
                ClassName = $"{@class}Client",
                OperationNameGenerator = new MultipleClientsFromPathSegmentsOperationNameGenerator(),
                ClientClassAccessModifier = "public",
                UseBaseUrl = false,
                InjectHttpClient = true,
                ExceptionClass = "ApiException",
                GenerateClientInterfaces = true,
                GenerateOptionalParameters = true,
                ExposeJsonSerializerSettings = true,
                CSharpGeneratorSettings =
                {
                    Namespace = @namespace,
                    JsonLibrary = CSharpJsonLibrary.SystemTextJson,
                    ClassStyle = CSharpClassStyle.Record,
                    GenerateDefaultValues = true,
                    RequiredPropertiesMustBeDefined = true,
                    GenerateNullableReferenceTypes = true
                }
            });

            return generator.GenerateFile();
        }
        catch (Exception ex)
        {
            throw new DiagnosticException("010", $"Can't generate code from Open API Specification: {ex.Message}");
        }
    }
}