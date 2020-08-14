using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    /// <summary>
    /// A name of a variable or namespace
    /// </summary>
    internal class NameExpressionSyntax : ExpressionSyntax, INameExpressionSyntax
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
        // A null name means this syntax was generated as an assumed missing name and the name is unknown
        public Name? Name { get; }
        public Promise<BindingSymbol?> ReferencedSymbol { get; } = new Promise<BindingSymbol?>();
        public bool VariableIsLiveAfter { get; set; } = true;

        public NameExpressionSyntax(TextSpan span, Name? name)
            : base(span)
        {
            Name = name;
        }

        public IEnumerable<IPromise<BindingSymbol>> LookupInContainingScope()
        {
            if (containingLexicalScope == null)
                throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");

            // If name is unknown, no symbols
            if (Name is null) return Enumerable.Empty<IPromise<BindingSymbol>>();

            return containingLexicalScope.Lookup(Name).Select(p => p.As<BindingSymbol>()).NotNull();
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
        public override string ToString()
        {
            return Name?.ToString() ?? "⧼unknown⧽";
        }
    }
}
