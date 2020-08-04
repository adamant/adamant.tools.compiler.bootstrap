using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class SelfExpressionSyntax : ExpressionSyntax, ISelfExpressionSyntax
    {
        public bool IsImplicit { get; }

        private IBindingMetadata? referencedSymbol;
        [DisallowNull]
        public IBindingMetadata? ReferencedBinding
        {
            get => referencedSymbol;
            set
            {
                if (referencedSymbol != null)
                    throw new InvalidOperationException("Can't set referenced symbol repeatedly");
                referencedSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        private LexicalScope? containingScope;
        [DisallowNull]
        public LexicalScope? ContainingScope
        {
            [DebuggerStepThrough]
            get => containingScope;
            set
            {
                if (containingScope != null)
                    throw new InvalidOperationException("Can't set containing scope repeatedly");
                containingScope = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public SelfExpressionSyntax(TextSpan span, bool isImplicit)
            : base(span)
        {
            IsImplicit = isImplicit;
        }

        public FixedList<IMetadata> LookupInContainingScope()
        {
            if (ContainingScope != null) return ContainingScope.LookupMetadata(SpecialNames.Self);
            throw new InvalidOperationException();
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return IsImplicit ? "⟦self⟧" : "self";
        }
    }
}
