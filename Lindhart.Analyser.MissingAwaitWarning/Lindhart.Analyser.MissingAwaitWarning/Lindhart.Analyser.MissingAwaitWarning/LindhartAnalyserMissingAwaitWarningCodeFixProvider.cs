using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;

namespace Lindhart.Analyser.MissingAwaitWarning
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LindhartAnalyserMissingAwaitWarningCodeFixProvider)), Shared]
    public class LindhartAnalyserMissingAwaitWarningCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Insert 'await' keyword";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            LindhartAnalyserMissingAwaitWarningAnalyzer.StandardRuleId,
            LindhartAnalyserMissingAwaitWarningAnalyzer.StrictRuleId // This use case is already covered by a standard roslyn code fix. Should we still add our for coherence?
            );

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            ExpressionSyntax declaration;
            switch (diagnostic.Id)
            {
                case LindhartAnalyserMissingAwaitWarningAnalyzer.StandardRuleId:
                    declaration = root.FindToken(diagnosticSpan.Start)
                                      .Parent
                                      .AncestorsAndSelf()
                                      .OfType<InvocationExpressionSyntax>()
                                      .First();
                    break;
                case LindhartAnalyserMissingAwaitWarningAnalyzer.StrictRuleId:
                    declaration = root.FindToken(diagnosticSpan.Start)
                                      .Parent
                                      .AncestorsAndSelf()
                                      .OfType<VariableDeclarationSyntax>()
                                      .First()
                                      .DescendantNodes()
                                      .OfType<EqualsValueClauseSyntax>()
                                      .First()
                                      .DescendantNodes()
                                      .OfType<ExpressionSyntax>()
                                      .First();
                    break;
                default:
                    return;
            }

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                    CodeAction.Create(
                            title: Title,
                            createChangedDocument: c => InsertAwaitKeyword(context.Document, declaration, c),
                            equivalenceKey: Title),
                    diagnostic);
        }

        private async Task<Document> InsertAwaitKeyword(Document document, ExpressionSyntax declaration, CancellationToken cancellationToken)
        {
            // Get comments (comments and stuff)
            var firstToken = declaration.GetFirstToken();
            var leadingTrivia = firstToken.LeadingTrivia;
            // Remove comments
            var newDeclaration = declaration.ReplaceToken(firstToken, firstToken.WithLeadingTrivia(SyntaxTriviaList.Empty));

            ExpressionSyntax parentesized = SyntaxFactory
                .ParenthesizedExpression(newDeclaration)
                .WithAdditionalAnnotations(Simplifier.Annotation);

            // Create 'await' expression before the problem
            AwaitExpressionSyntax awaiter = SyntaxFactory.AwaitExpression(parentesized);

            // Replace the node with the new node - and insert trivia on the new node
            var rootNode = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = rootNode.ReplaceNode(declaration, awaiter.WithLeadingTrivia(leadingTrivia));

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
