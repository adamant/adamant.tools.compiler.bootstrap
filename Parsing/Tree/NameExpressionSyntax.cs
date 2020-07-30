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
    /// <summary>
    /// A name of a variable or namespace
    /// </summary>
    internal class NameExpressionSyntax : ExpressionSyntax, INameExpressionSyntax
    {
        public SimpleName Name { [DebuggerStepThrough] get; }

        private IBindingSymbol? referencedSymbol;
        [DisallowNull]
        public IBindingSymbol? ReferencedSymbol
        {
            [DebuggerStepThrough]
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

        public bool VariableIsLiveAfter { get; set; } = true;

        public NameExpressionSyntax(TextSpan span, SimpleName name)
            : base(span)
        {
            Name = name;
        }

        public FixedList<ISymbol> LookupInContainingScope()
        {
            if (ContainingScope != null)
                return ContainingScope.Lookup(Name);
            throw new InvalidOperationException();
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
