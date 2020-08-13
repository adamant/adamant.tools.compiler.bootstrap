using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.InferredSyntax
{
    internal class InvocableNameSyntax : IInvocableNameSyntax
    {
        public TextSpan Span { get; }
        public MaybeQualifiedName Name { get; }

        private readonly Scope containingScope;
        [DisallowNull]
        public Scope? ContainingScope
        {
            get => containingScope;
            set => throw new InvalidOperationException("Can't set containing scope repeatedly");
        }

        public InvocableNameSyntax(TextSpan span, MaybeQualifiedName name, Scope containingScope)
        {
            Span = span;
            Name = name;
            this.containingScope = containingScope;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
