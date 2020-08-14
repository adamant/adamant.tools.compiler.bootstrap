using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
        public new Name Name { get; }
        public new Promise<ObjectTypeSymbol> Symbol { get; }
        public FixedList<IMemberDeclarationSyntax> Members { get; }
        public ConstructorSymbol? DefaultConstructorSymbol { get; private set; }

        public ClassDeclarationSyntax(
            NamespaceName containingNamespaceName,
            TextSpan headerSpan,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            IMutableKeywordToken? mutableModifier,
            TextSpan nameSpan,
            Name name,
            Func<IClassDeclarationSyntax, (FixedList<IMemberDeclarationSyntax>, TextSpan)> parseMembers)
            : base(headerSpan, file, name, nameSpan, new Promise<ObjectTypeSymbol>())
        {
            ContainingNamespaceName = containingNamespaceName;
            AccessModifier = accessModifier;
            MutableModifier = mutableModifier;
            Name = name;
            var (members, bodySpan) = parseMembers(this);
            Members = members;
            Span = TextSpan.Covering(headerSpan, bodySpan);
            Symbol = (Promise<ObjectTypeSymbol>)base.Symbol;
        }

        public void CreateDefaultConstructor(SymbolTreeBuilder symbolTree)
        {
            if (Members.Any(m => m is IConstructorDeclarationSyntax))
                return;

            if (!(DefaultConstructorSymbol is null))
                throw new InvalidOperationException($"Can't {nameof(CreateDefaultConstructor)} twice");

            var constructedType = Symbol.Result.DeclaresDataType;
            var constructorSymbol = new ConstructorSymbol(Symbol.Result, null, FixedList<DataType>.Empty);
            var selfParameterSymbol = new SelfParameterSymbol(constructorSymbol, false, constructedType);

            symbolTree.Add(constructorSymbol);
            symbolTree.Add(selfParameterSymbol);
            DefaultConstructorSymbol = constructorSymbol;
        }

        public override string ToString()
        {
            return $"class {Name} {{ … }}";
        }
    }
}
