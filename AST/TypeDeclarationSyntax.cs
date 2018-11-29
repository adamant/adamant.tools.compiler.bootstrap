using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class TypeDeclarationSyntax : DeclarationSyntax, INamespacedDeclarationSyntax, IMemberDeclarationSyntax, ISymbol
    {
        [NotNull] public Name FullName { get; }
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [NotNull] public FixedList<IMemberDeclarationSyntax> Members { get; }
        [NotNull] public TypePromise<Metatype> Type { get; } = new TypePromise<Metatype>();

        DataType ISymbol.Type => Type.Fulfilled();
        public FixedDictionary<SimpleName, ISymbol> ChildSymbols { get; }

        protected TypeDeclarationSyntax(
            [NotNull] CodeFile file,
            TextSpan nameSpan,
            [NotNull] Name fullName,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] FixedList<IMemberDeclarationSyntax> members)
            : base(file, nameSpan)
        {
            FullName = fullName;
            Members = members;
            GenericParameters = genericParameters;
            ChildSymbols = members.ToFixedDictionary(m => m.FullName.UnqualifiedName, m => (ISymbol)m);
        }

        DeclarationSyntax IDeclarationSyntax.AsDeclarationSyntax => this;
    }
}
