using System;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ClassDeclarationSyntax : DeclarationSyntax, IClassDeclarationSyntax
    {
        private LexicalScope<Promise<Symbol?>>? containingLexicalScope;
        public LexicalScope<Promise<Symbol?>>? ContainingLexicalScope
        {
            get => containingLexicalScope;
            set
            {
                if (containingLexicalScope != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingLexicalScope)} repeatedly");
                containingLexicalScope = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public NamespaceName ContainingNamespaceName { get; }

        private NamespaceOrPackageSymbol? containingNamespaceSymbol;
        public NamespaceOrPackageSymbol ContainingNamespaceSymbol
        {
            get => containingNamespaceSymbol
                   ?? throw new InvalidOperationException($"{ContainingNamespaceSymbol} not yet assigned");
            set
            {
                if (containingNamespaceSymbol != null)
                    throw new InvalidOperationException($"Can't set {nameof(ContainingNamespaceSymbol)} repeatedly");
                containingNamespaceSymbol = value;
            }
        }

        public IAccessModifierToken? AccessModifier { get; }
        public IMutableKeywordToken? MutableModifier { get; }
        public MaybeQualifiedName FullName { get; }
        public Name Name { get; }
        public FixedList<IMemberDeclarationSyntax> Members { get; }
        public DataTypePromise DeclaresDataType { get; } = new DataTypePromise();
        public MetadataSet ChildMetadata { get; protected set; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ITypeMetadata.DeclaresDataType => DeclaresDataType.Fulfilled();

        public ClassDeclarationSyntax(
            NamespaceName containingNamespaceName,
            TextSpan headerSpan,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            IMutableKeywordToken? mutableModifier,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            Name name,
            Func<IClassDeclarationSyntax, (FixedList<IMemberDeclarationSyntax>, TextSpan)> parseMembers)
            : base(headerSpan, file, nameSpan)
        {
            ContainingNamespaceName = containingNamespaceName;
            AccessModifier = accessModifier;
            MutableModifier = mutableModifier;
            FullName = fullName;
            Name = name;
            var (members, bodySpan) = parseMembers(this);
            Members = members;
            ChildMetadata = new MetadataSet(Members);
            Span = TextSpan.Covering(headerSpan, bodySpan);
        }

        public void CreateDefaultConstructor()
        {
            if (Members.Any(m => m is IConstructorDeclarationSyntax))
                return;

            var constructor = new DefaultConstructor((ObjectType)DeclaresDataType.Fulfilled());
            ChildMetadata = new MetadataSet(ChildMetadata.Append<IMetadata>(constructor));
        }

        public override string ToString()
        {
            return $"class {FullName} {{ … }}";
        }
    }
}
