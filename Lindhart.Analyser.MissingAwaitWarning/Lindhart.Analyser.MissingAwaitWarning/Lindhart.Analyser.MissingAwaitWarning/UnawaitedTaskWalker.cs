using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lindhart.Analyser.MissingAwaitWarning
{
    internal class UnawaitedTaskWalker : CSharpSyntaxWalker
    {
        private readonly CodeBlockAnalysisContext _context;
        private Dictionary<ISymbol, bool> _awaitedTasks;

        public IReadOnlyCollection<ISymbol> UnawaitedTasks =>
            _awaitedTasks.Where(x => x.Value == false)
                         .Select(x => x.Key)
                         .ToList();

        public UnawaitedTaskWalker(CodeBlockAnalysisContext context)
        {
            _context = context;
            _awaitedTasks = new Dictionary<ISymbol, bool>();
        }

        public void Analyze(SyntaxNode node)
        {
            this.Visit(node);
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            base.VisitVariableDeclarator(node);

            var declaredSymbol = _context.SemanticModel.GetDeclaredSymbol(node);
            if (declaredSymbol is ILocalSymbol localSymbol)
            {
                if (Common.EqualsType(localSymbol.Type, _context.SemanticModel, Common.AwaitableTypes))
                {
                    if (!_awaitedTasks.ContainsKey(declaredSymbol))
                        _awaitedTasks[declaredSymbol] = false;
                }
            }
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            base.VisitAwaitExpression(node);

            var awaitedTask = _context.SemanticModel.GetSymbolInfo(node.Expression).Symbol;
            if (awaitedTask is ILocalSymbol)
            {
                _awaitedTasks[awaitedTask] = true;
            }
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            base.VisitReturnStatement(node);

            var awaitedTask = _context.SemanticModel.GetSymbolInfo(node.Expression).Symbol;
            if (awaitedTask is ILocalSymbol)
            {
                _awaitedTasks[awaitedTask] = true;
            }
        }
    }
}
