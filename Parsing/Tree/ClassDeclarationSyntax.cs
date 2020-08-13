using System;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ClassDeclarationSyntax : DeclarationSyntax, IClassDeclarationSyntax
    {
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
        public new Name Name { get; }
        public Promise<ObjectTypeSymbol> Symbol { get; } = new Promise<ObjectTypeSymbol>();
        protected override IPromise<Symbol> SymbolPromise => Symbol;
        public FixedList<IMemberDeclarationSyntax> Members { get; }
        public MetadataSet ChildMetadata { get; protected set; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ITypeMetadata.DeclaresDataType => Symbol.Result.DeclaresDataType;

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
            : base(headerSpan, file, name, nameSpan)
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

        public void CreateDefaultConstructor(SymbolTreeBuilder symbolTree)
        {
            if (Members.Any(m => m is IConstructorDeclarationSyntax))
                return;

            var constructedType = Symbol.Result.DeclaresDataType;
            var constructorSymbol = new ConstructorSymbol(Symbol.Result, null, FixedList<DataType>.Empty);
            var selfParameterSymbol = new SelfParameterSymbol(constructorSymbol, false, constructedType);

            var constructor = new DefaultConstructor(constructorSymbol, selfParameterSymbol);
            symbolTree.Add(constructorSymbol);
            symbolTree.Add(selfParameterSymbol);

            ChildMetadata = new MetadataSet(ChildMetadata.Append<IMetadata>(constructor));
        }

        public override string ToString()
        {
            return $"class {FullName} {{ … }}";
        }
    }
}
