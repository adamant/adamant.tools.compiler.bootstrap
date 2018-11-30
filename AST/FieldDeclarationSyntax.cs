using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class FieldDeclarationSyntax : DeclarationSyntax, IMemberDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] [ItemNotNull] public FixedList<IModiferToken> Modifiers { get; }
        public AccessModifier? GetterAccess { get; }
        [NotNull] public Name FullName { get; }
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] DataType ISymbol.Type => Type.Fulfilled();
        FixedDictionary<SimpleName, ISymbol> ISymbol.ChildSymbols => FixedDictionary<SimpleName, ISymbol>.Empty;

        public FieldDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] [ItemNotNull] FixedList<IModiferToken> modifiers,
            AccessModifier? getterAccess,
            [NotNull] Name fullName,
            TextSpan nameSpan,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] ExpressionSyntax initializer)
            : base(file, nameSpan)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            GetterAccess = getterAccess;
            FullName = fullName;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DeclarationSyntax IDeclarationSyntax.AsDeclarationSyntax => this;

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
