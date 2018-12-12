using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lindhart.Analyser.MissingAwaitWarning
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LindhartAnalyserMissingAwaitWarningAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "LindhartAnalyserMissingAwaitWarning";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "UnintentionalUsage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        private static readonly Type[] AwaitableTypes = new[]
        {
            typeof(Task),
            typeof(Task<>),
            typeof(ConfiguredTaskAwaitable),
            typeof(ConfiguredTaskAwaitable<>),
            typeof(ValueTask),
            typeof(ValueTask<>),
            typeof(ConfiguredValueTaskAwaitable),
            typeof(ConfiguredValueTaskAwaitable<>)
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyseSymbolNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyseSymbolNode(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            if (syntaxNodeAnalysisContext.Node is InvocationExpressionSyntax node)
            {
                var symbolInfo = syntaxNodeAnalysisContext
                    .SemanticModel
                    .GetSymbolInfo(node.Expression, syntaxNodeAnalysisContext.CancellationToken);
                
                if ((symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault())
                    is IMethodSymbol methodSymbol)
                {
                    if (node.Parent is ExpressionStatementSyntax)
                    {
                        // Only checks for the two most common awaitable types. In principle this should instead check all types that are awaitable
                        if (EqualsType(methodSymbol.ReturnType, syntaxNodeAnalysisContext.SemanticModel, AwaitableTypes))
                        {
                            var diagnostic = Diagnostic.Create(Rule, node.GetLocation(), methodSymbol.ToDisplayString());

                            syntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the <paramref name="typeSymbol"/> is one of the types specified
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="semanticModel">Semantic Model of the current context</param>
        /// <param name="types">List of parameters that should match the symbol's type</param>
        /// <returns></returns>
        private static bool EqualsType(ITypeSymbol typeSymbol, SemanticModel semanticModel, params Type[] types)
        {
            var namedTypeSymbols = types.Select(x => semanticModel.Compilation.GetTypeByMetadataName(x.FullName));

            var namedSymbol = typeSymbol as INamedTypeSymbol;
            if (namedSymbol == null)
                return false;

            if (namedSymbol.IsGenericType)
                return namedTypeSymbols.Any(t => namedSymbol.ConstructedFrom.Equals(t));
            else
                return namedTypeSymbols.Any(t => typeSymbol.Equals(t));
        }
    }
}