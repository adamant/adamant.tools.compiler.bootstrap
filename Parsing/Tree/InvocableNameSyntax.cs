using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class InvocableNameSyntax : Syntax, IInvocableNameSyntax
    {
        public MaybeQualifiedName Name { get; }

        private IFunctionMetadata? referencedFunctionMetadata;

        [DisallowNull]
        public IFunctionMetadata? ReferencedFunctionMetadata
        {
            get => referencedFunctionMetadata;
            set
            {
                if (referencedFunctionMetadata != null)
                    throw new InvalidOperationException($"Can't set {nameof(ReferencedFunctionMetadata)} repeatedly");
                referencedFunctionMetadata = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        private Scope? containingScope;

        [DisallowNull]
        public Scope? ContainingScope
        {
            get => containingScope;
            set
            {
                if (containingScope != null)
                    throw new InvalidOperationException("Can't set containing scope repeatedly");
                containingScope = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public InvocableNameSyntax(TextSpan span, SimpleName name)
            : base(span)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}