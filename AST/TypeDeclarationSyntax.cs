using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class TypeDeclarationSyntax : MemberDeclarationSyntax, INamespacedDeclarationSyntax
    {
        [CanBeNull] public FixedList<GenericParameterSyntax> GenericParameters { get; }
        [NotNull] public FixedList<MemberDeclarationSyntax> Members { get; }
        [NotNull] public TypePromise<Metatype> Type { get; } = new TypePromise<Metatype>();

        protected TypeDeclarationSyntax(
            [NotNull] CodeFile file,
            TextSpan nameSpan,
            [NotNull] Name fullName,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] FixedList<MemberDeclarationSyntax> members)
            : base(file, fullName, nameSpan, new SymbolSet(members))
        {
            Members = members;
            GenericParameters = genericParameters;
        }

        protected override DataType GetDataType()
        {
            return Type.Fulfilled();
        }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DeclarationSyntax IDeclarationSyntax.AsDeclarationSyntax => this;

    }
}
