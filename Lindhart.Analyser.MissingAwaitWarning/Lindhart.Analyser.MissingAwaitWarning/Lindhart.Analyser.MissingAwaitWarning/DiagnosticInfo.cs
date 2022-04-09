using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lindhart.Analyser.MissingAwaitWarning
{
    public readonly struct DiagnosticInfo
    {
        public DiagnosticInfo(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode node, IMethodSymbol methodSymbol)
        {
            SyntaxNodeAnalysisContext = syntaxNodeAnalysisContext;
            Node = node;
            MethodSymbol = methodSymbol;
        }

        public SyntaxNodeAnalysisContext SyntaxNodeAnalysisContext { get; }
        public SyntaxNode Node { get; }
        public IMethodSymbol MethodSymbol { get; }

        public DiagnosticInfo ReplaceNode(SyntaxNode newNode)
        {
            return new DiagnosticInfo(SyntaxNodeAnalysisContext, newNode, MethodSymbol);
        }
    }
}