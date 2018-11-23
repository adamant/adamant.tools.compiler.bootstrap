using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class TypeDeclarationSyntax : DeclarationSyntax, INamespacedDeclarationSyntax, IMemberDeclarationSyntax, ISymbol
    {
        [NotNull] public Name FullName { get; }
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [NotNull] public FixedList<IMemberDeclarationSyntax> Members { get; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();
        DataType ISymbol.Type => Type.Fulfilled();

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
        }

        public ISymbol ComposeWith(ISymbol symbol)
        {
            throw new System.NotImplementedException();
        }

        public ISymbol Lookup(SimpleName name)
        {
            throw new System.NotImplementedException();
        }

        ISymbol ISymbol.ComposeWith(ISymbol symbol)
        {
            throw new System.NotImplementedException();
        }

        ISymbol ISymbol.Lookup(SimpleName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
