using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class QualifiedInvocationExpressionSyntax : InvocationExpressionSyntax, IQualifiedInvocationExpressionSyntax
    {
        private LexicalScope? containingLexicalScope;
        public LexicalScope ContainingLexicalScope
        {
            [DebuggerStepThrough]
            get =>
                containingLexicalScope
                ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
            [DebuggerStepThrough]
            set
            {
                if (containingLexicalScope != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
                containingLexicalScope = value;
            }
        }

        private IExpressionSyntax context;
        public ref IExpressionSyntax Context => ref context;
        public new Promise<MethodSymbol?> ReferencedSymbol { get; }

        public QualifiedInvocationExpressionSyntax(
            TextSpan span,
            IExpressionSyntax context,
            Name invokedName,
            TextSpan invokedNameSpan,
            FixedList<IArgumentSyntax> arguments)
            : base(span, invokedName, invokedNameSpan, arguments, new Promise<MethodSymbol?>())
        {
            this.context = context;
            ReferencedSymbol = (Promise<MethodSymbol?>)base.ReferencedSymbol;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{Context.ToGroupedString(ExpressionPrecedence)}.{InvokedName}({string.Join(", ", Arguments)})";
        }
    }
}
