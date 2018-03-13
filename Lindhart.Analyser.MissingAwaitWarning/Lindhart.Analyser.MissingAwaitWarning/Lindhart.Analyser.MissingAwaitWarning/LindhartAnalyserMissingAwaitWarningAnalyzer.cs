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

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyseSymbolNode, SyntaxKind.InvocationExpression);
		}

		private void AnalyseSymbolNode(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
		{
			if (syntaxNodeAnalysisContext.Node is InvocationExpressionSyntax node)
			{
				if (syntaxNodeAnalysisContext
						.SemanticModel
						.GetSymbolInfo(node.Expression, syntaxNodeAnalysisContext.CancellationToken)
						.Symbol is IMethodSymbol methodSymbol)
				{
					if (node.Parent is ExpressionStatementSyntax)
					{
						// Only checks for the two most common awaitable types. In principle this should instead check all types that are awaitable
						if (EqualsType(methodSymbol.ReturnType, typeof(Task), typeof(ConfiguredTaskAwaitable)))
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
		/// <param name="type"></param>
		/// <returns></returns>
		/// <remarks>This method should probably be rewritten so it doesn't merely compare the names, but instead the actual type.</remarks>
		private static bool EqualsType(ITypeSymbol typeSymbol, params Type[] type)
		{
			// ContaningNamespace can be null, see https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/Symbols/ISymbol.cs#L76
			var fullSymbolNameWithoutGeneric = typeSymbol.ContainingNamespace == null ? typeSymbol.Name : $"{typeSymbol.ContainingNamespace.ToDisplayString()}.{typeSymbol.Name}";
			return type.Any(x => fullSymbolNameWithoutGeneric.Equals(x.FullName));
		}
	}
}
