using System.Diagnostics;
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
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public SimpleName LookupByName => FullName.UnqualifiedName;
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [NotNull] public FixedList<IMemberDeclarationSyntax> Members { get; }
        [NotNull] public TypePromise<Metatype> Type { get; } = new TypePromise<Metatype>();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ISymbol.Type => Type.Fulfilled();
        public SymbolSet ChildSymbols { get; }

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
            ChildSymbols = new SymbolSet(members);
        }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DeclarationSyntax IDeclarationSyntax.AsDeclarationSyntax => this;
    }
}
