using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riverside.Extensions.WinUI
{
    [Generator]
    public partial class InitializeComponentGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this source generator
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Find all classes with the InitializeComponentAttribute
            var classesWithAttribute = context.Compilation.SyntaxTrees
                .SelectMany(syntaxTree => syntaxTree.GetRoot().DescendantNodes())
                .OfType<ClassDeclarationSyntax>()
                .Where(classDeclaration => classDeclaration.AttributeLists
                    .SelectMany(attributeList => attributeList.Attributes)
                    .Any(attribute => attribute.Name.ToString() == "InitializeComponent"));

            foreach (var classDeclaration in classesWithAttribute)
            {
                var namespaceDeclaration = classDeclaration.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                var namespaceName = namespaceDeclaration?.Name.ToString() ?? "GlobalNamespace";
                var className = classDeclaration.Identifier.Text;

                var source = $@"
namespace {namespaceName}
{{
    public partial class {className}
    {{
        public {className}()
        {{
            this.InitializeComponent();
        }}
    }}
}}";
                context.AddSource($"{className}_InitializeComponent.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
    }
}
