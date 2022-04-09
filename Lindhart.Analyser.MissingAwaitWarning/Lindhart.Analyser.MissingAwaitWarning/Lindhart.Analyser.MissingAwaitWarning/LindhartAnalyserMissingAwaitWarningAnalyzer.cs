using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Lindhart.Analyser.MissingAwaitWarning
{
    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public class LindhartAnalyserMissingAwaitWarningAnalyzer : DiagnosticAnalyzer
    {
        public const string UnawaitedTaskRuleId = "LindhartAnalyserMissingAwaitWarning";
        public const string PossibleUnawaitedTaskVariableRuleId = "LindhartAnalyserMissingAwaitWarningVariable";
        public const string UnawaitedAnonymousFunctionRuleId = "LindhartAnalyserMissingAwaitWarningVariable";
        public const string UnawaitedLambdaFunctionRuleId = "LindhartAnalyserMissingAwaitWarningVariable";

        private const string Category = "UnintentionalUsage";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        public static readonly LocalizableString UnawaitedTaskTitle = new LocalizableResourceString( nameof( Resources.UnawaitedTaskRuleTitle ), Resources.ResourceManager, typeof( Resources ) );
        public static readonly LocalizableString UnawaitedTaskMessageFormat = new LocalizableResourceString( nameof( Resources.UnawaitedTaskMessageFormat ), Resources.ResourceManager, typeof( Resources ) );
        public static readonly LocalizableString UnawaitedTaskDescription = new LocalizableResourceString( nameof( Resources.UnawaitedTaskDescription ), Resources.ResourceManager, typeof( Resources ) );

        public static readonly LocalizableString PossibleUnawaitedVariableTitle = new LocalizableResourceString(nameof(Resources.PossibleUnawaitedVaraibleRuleTitle), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString PossibleUnawaitedVariableMessageFormat = new LocalizableResourceString(nameof(Resources.PossibleUnawaitedVariableMessageFormat), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString PossibleUnawaitedVariableDescription = new LocalizableResourceString(nameof(Resources.PossibleUnawaitedVariableDescription), Resources.ResourceManager, typeof(Resources));
        
        public static readonly LocalizableString UnawaitedAnonymousFunctionTitle = new LocalizableResourceString(nameof(Resources.PossibleUnawaitedVaraibleRuleTitle), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString UnawaitedAnonymousFunctionFormat = new LocalizableResourceString(nameof(Resources.PossibleUnawaitedVariableMessageFormat), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString UnawaitedAnonymousFunctionDescription = new LocalizableResourceString(nameof(Resources.PossibleUnawaitedVariableDescription), Resources.ResourceManager, typeof(Resources));

        public static readonly LocalizableString UnawaitedLambdaFunctionTitle = new LocalizableResourceString(nameof(Resources.PossibleUnawaitedVaraibleRuleTitle), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString UnawaitedLambdaFunctionFormat = new LocalizableResourceString(nameof(Resources.PossibleUnawaitedVariableMessageFormat), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString UnawaitedLambdaFunctionDescription = new LocalizableResourceString(nameof(Resources.PossibleUnawaitedVariableDescription), Resources.ResourceManager, typeof(Resources));


        private static readonly DiagnosticDescriptor UnawaitedTaskRule = new DiagnosticDescriptor( UnawaitedTaskRuleId, UnawaitedTaskTitle, UnawaitedTaskMessageFormat, Category, DiagnosticSeverity.Warning, true, UnawaitedTaskDescription );
        private static readonly DiagnosticDescriptor PossibleUnawaitedVariableRule = new DiagnosticDescriptor( PossibleUnawaitedTaskVariableRuleId, PossibleUnawaitedVariableTitle, PossibleUnawaitedVariableMessageFormat, Category, DiagnosticSeverity.Hidden, false, PossibleUnawaitedVariableDescription );
        private static readonly DiagnosticDescriptor UnawaitedAnonymousFunctionRule = new DiagnosticDescriptor(PossibleUnawaitedTaskVariableRuleId, PossibleUnawaitedVariableTitle, PossibleUnawaitedVariableMessageFormat, Category, DiagnosticSeverity.Hidden, false, PossibleUnawaitedVariableDescription);
        private static readonly DiagnosticDescriptor UnawaitedLambdaFunctionRule = new DiagnosticDescriptor(PossibleUnawaitedTaskVariableRuleId, PossibleUnawaitedVariableTitle, PossibleUnawaitedVariableMessageFormat, Category, DiagnosticSeverity.Hidden, false, PossibleUnawaitedVariableDescription);



        private static readonly string[] AwaitableTypes = new[]
        {
            typeof(Task).FullName,
            typeof(Task<>).FullName,
            typeof(ConfiguredTaskAwaitable).FullName,
            typeof(ConfiguredTaskAwaitable<>).FullName,
            "System.Threading.Tasks.ValueTask", // Type not available in .net standard 1.3
            typeof(ValueTask<>).FullName,
            "System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable", // Type not available in .net standard
            typeof(ConfiguredValueTaskAwaitable<>).FullName
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( UnawaitedTaskRule, PossibleUnawaitedVariableRule );

        public override void Initialize( AnalysisContext context )
        {
            context.RegisterSyntaxNodeAction( AnalyseSymbolNode, SyntaxKind.InvocationExpression );
        }

        private void AnalyseSymbolNode( SyntaxNodeAnalysisContext syntaxNodeAnalysisContext )
        {
            if ( syntaxNodeAnalysisContext.Node is InvocationExpressionSyntax node )
            {
                var symbolInfo = syntaxNodeAnalysisContext
                    .SemanticModel
                    .GetSymbolInfo( node.Expression, syntaxNodeAnalysisContext.CancellationToken );

                if ( ( symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault() )
                    is IMethodSymbol methodSymbol )
                {
                    AnalyseParentNode(new DiagnosticInfo(syntaxNodeAnalysisContext, node, methodSymbol));
                }
            }
        }

        private static void AnalyseParentNode(DiagnosticInfo info )
        {
            switch ( info.Node.Parent )
            {
                // Checks if a task is not awaited when the task itself is not assigned to a variable.
                case ExpressionStatementSyntax _:
                    // Check the method return type against all the known awaitable types.
                    if ( IsAwaitableType( info.MethodSymbol.ReturnType, info.SyntaxNodeAnalysisContext.SemanticModel) )
                    {
                        ReportDiagnostic(info, UnawaitedTaskRule);
                    }

                    break;

                // Checks if a task is not awaited in lambdas.
                case AnonymousFunctionExpressionSyntax _:
                    TestAnonymousFunctionExpression(info);
                    break;
                case ArrowExpressionClauseSyntax _:
                    TestLambdaFunction(info);
                    break;
                // Checks if a task is not awaited when the task itself is assigned to a variable.
                case EqualsValueClauseSyntax _:
                    TestAssignedVariableIsUnawaitedTask(info);
                    break;

                // If the conditional expression, we check recursively
                case ConditionalAccessExpressionSyntax _:
                    AnalyseParentNode(info.ReplaceNode(info.Node.Parent));
                    break;

                // Awaited expressions don't interest us
                case AwaitExpressionSyntax _:

                    break;
            }
        }

        private static void TestAnonymousFunctionExpression(DiagnosticInfo info)
        {
            if (IsAwaitableType(info.MethodSymbol.ReturnType, info.SyntaxNodeAnalysisContext.SemanticModel))
            {
                
                if (info.Node.Parent is ParenthesizedLambdaExpressionSyntax { ReturnType: IdentifierNameSyntax back })
                    return;

                // If the anonymous function declaration returns a Task then we allow the lampda expression to do the same
                var possibleMethodParent = info.Node.Parent?.Parent?.Parent?.Parent?.Parent;

                // This could indicate that the anonymous function is a delegate so we check if the underlying delegate returns a task
                if (possibleMethodParent is LocalDeclarationStatementSyntax lambda)
                    possibleMethodParent = lambda.Parent?.Parent;

                if (possibleMethodParent is MethodDeclarationSyntax method &&
                    method.ReturnType is IdentifierNameSyntax returnType)
                {
                    if (IsAwaitableType(info.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(returnType).Type,
                            info.SyntaxNodeAnalysisContext.SemanticModel))
                        return;
                }

                ReportDiagnostic(info, PossibleUnawaitedVariableRule);//AnonymousFunctionRule);
            }
        }

        private static void TestLambdaFunction(DiagnosticInfo info)
        {
            if (IsAwaitableType(info.MethodSymbol.ReturnType, info.SyntaxNodeAnalysisContext.SemanticModel))
            {
                // If the local function returns a kind of a Task, then it is ok that the local function also does it
                if (info.Node.Parent?.Parent is LocalFunctionStatementSyntax function && function.ReturnType is IdentifierNameSyntax returnType)
                {
                    if (IsAwaitableType(info.SyntaxNodeAnalysisContext.SemanticModel.GetTypeInfo(returnType).Type,
                            info.SyntaxNodeAnalysisContext.SemanticModel))
                        return;
                }

                ReportDiagnostic(info, PossibleUnawaitedVariableRule); //LambdaFunctionRule);
            }
        }

        private static void TestAssignedVariableIsUnawaitedTask(DiagnosticInfo info)
        {
            if (IsAwaitableType(info.MethodSymbol.ReturnType, info.SyntaxNodeAnalysisContext.SemanticModel))
            {
                ReportDiagnostic(info, PossibleUnawaitedVariableRule);
            }
        }

        private static void ReportDiagnostic(DiagnosticInfo info, DiagnosticDescriptor rule)
        {
            var diagnostic = Diagnostic.Create(rule, info.Node.GetLocation(),
                info.MethodSymbol.ToDisplayString());
            info.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
        }

        /// <summary>
        /// Checks if the <paramref name="typeSymbol"/> is one of the awaitable types
        /// </summary>
        /// <param name="typeSymbol">Type to check</param>
        /// <param name="semanticModel">Semantic Model of the current context</param>
        /// <returns></returns>
        private static bool IsAwaitableType(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            var namedTypeSymbols = AwaitableTypes.Select( x => semanticModel.Compilation.GetTypeByMetadataName( x ) );

            if ( !(typeSymbol is INamedTypeSymbol namedSymbol) )
                return false;

            if ( namedSymbol.IsGenericType )
                return namedTypeSymbols.Any( t => namedSymbol.ConstructedFrom.Equals( t ) );

            return namedTypeSymbols.Any( typeSymbol.Equals );
        }
    }
}