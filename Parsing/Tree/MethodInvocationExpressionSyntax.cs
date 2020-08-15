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
    internal class MethodInvocationExpressionSyntax : InvocationExpressionSyntax, IMethodInvocationExpressionSyntax
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
        public IInvocableNameSyntax MethodNameSyntax { get; }
        public Promise<MethodSymbol?> ReferencedSymbol { get; } = new Promise<MethodSymbol?>();

        public MethodInvocationExpressionSyntax(
            TextSpan span,
            IExpressionSyntax context,
            Name name,
            IInvocableNameSyntax methodNameSyntax,
            FixedList<IArgumentSyntax> arguments)
            : base(span, name, arguments)
        {
            this.context = context;
            MethodNameSyntax = methodNameSyntax;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{Context.ToGroupedString(ExpressionPrecedence)}.{Name}({string.Join(", ", Arguments)})";
        }
    }
}
