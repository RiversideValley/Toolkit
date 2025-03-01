using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Riverside.SPDX;

/// <summary>
/// Source generator for creating an enum for every license in the SPDX project.
/// </summary>
[Generator]
public class SpdxProjectGenerator : IIncrementalGenerator
{
    private const string SpdxUrl = "https://spdx.org/licenses/licenses.json";

    /// <summary>
    /// Initializes the incremental generator.
    /// </summary>
    /// <param name="context">The initialization context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var licensesProvider = context.AdditionalTextsProvider
            .Select((additionalText, cancellationToken) => FetchSpdxLicenses().Result)
            .Collect();

        context.RegisterSourceOutput(licensesProvider, (spc, licenses) =>
        {
            var enumSource = GenerateEnumSource(licenses);
            spc.AddSource("SpdxLicense.g.cs", SourceText.From(enumSource, Encoding.UTF8));
        });
    }

    private async Task<List<string>> FetchSpdxLicenses()
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(SpdxUrl);
            var jsonDocument = JsonDocument.Parse(response);
            var licenses = new List<string>();

            foreach (var element in jsonDocument.RootElement.GetProperty("licenses").EnumerateArray())
            {
                var licenseId = element.GetProperty("licenseId").GetString();
                if (licenseId != null)
                {
                    licenses.Add(licenseId);
                }
            }

            Debug.WriteLine($"Fetched {licenses.Count} licenses: {string.Join(", ", licenses)}");
            return licenses;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Debug.WriteLine($"Exception in FetchSpdxLicenses: {ex}");
            return new List<string> { "None" }; // Return a default value to avoid breaking the generator
        }
    }

    private string GenerateEnumSource(ImmutableArray<List<string>> licensesArray)
    {
        var licenses = licensesArray.SelectMany(l => l).ToList();
        Debug.WriteLine($"Generating enum with {licenses.Count} licenses: {string.Join(", ", licenses)}");

        var enumMembers = new List<EnumMemberDeclarationSyntax>
        {
            SyntaxFactory.EnumMemberDeclaration("None")
        };

        foreach (var license in licenses)
        {
            var enumMember = SyntaxFactory.EnumMemberDeclaration(SanitizeEnumMemberName(license));
            enumMembers.Add(enumMember);
        }

        var enumDeclaration = SyntaxFactory.EnumDeclaration("License")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddMembers(enumMembers.ToArray());

        var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Riverside.SPDX"))
            .AddMembers(enumDeclaration);

        var compilationUnit = SyntaxFactory.CompilationUnit()
            .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
            .AddMembers(namespaceDeclaration);

        return compilationUnit.NormalizeWhitespace().ToFullString();
    }

    private string SanitizeEnumMemberName(string name)
    {
        return name.Replace("-", "_").Replace(".", "_");
    }
}
