using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    /// <summary>
    /// The unqualified name of a type
    /// </summary>
    internal class TypeNameSyntax : TypeSyntax, ITypeNameSyntax
    {
        private LexicalScope<IPromise<Symbol>>? containingLexicalScope;
        public LexicalScope<IPromise<Symbol>> ContainingLexicalScope
        {
            [DebuggerStepThrough]
            get => containingLexicalScope
                ?? throw new InvalidOperationException($"{nameof(ContainingLexicalScope)} not yet assigned");
            [DebuggerStepThrough]
            set
            {
                if (containingLexicalScope != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
                containingLexicalScope = value;
            }
        }
        public TypeName Name { get; }

        private IPromise<TypeSymbol?>? referencedSymbol;
        public IPromise<TypeSymbol?> ReferencedSymbol
        {
            get => referencedSymbol
                   ?? throw new InvalidOperationException($"{nameof(ReferencedSymbol)} not yet assigned");
            set
            {
                if (referencedSymbol != null)
                    throw new InvalidOperationException($"Can't set {nameof(ReferencedSymbol)} repeatedly");

                referencedSymbol = value;
            }
        }

        private IMetadata? referencedMetadata;

        [DisallowNull]
        public IMetadata? ReferencedMetadata
        {
            get => referencedMetadata;
            set
            {
                if (referencedMetadata != null)
                    throw new InvalidOperationException("Can't set referenced symbol repeatedly");
                referencedMetadata = value ?? throw new ArgumentNullException(nameof(value));
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

        public TypeNameSyntax(TextSpan span, TypeName name)
            : base(span)
        {
            Name = name;
        }

        public IEnumerable<IPromise<Symbol>> LookupInContainingScope()
        {
            if (containingLexicalScope != null)
                return containingLexicalScope.Lookup(Name);

            throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingLexicalScope)}");
        }

        public FixedList<IMetadata> LookupMetadataInContainingScope()
        {
            if (ContainingScope != null)
                return ContainingScope.LookupMetadata(Name.ToSimpleName());

            throw new InvalidOperationException($"Can't lookup type name without {nameof(ContainingScope)}");
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
