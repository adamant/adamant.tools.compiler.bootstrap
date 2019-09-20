using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(typeof(ClassDeclarationSyntax))]
    public abstract class TypeDeclarationSyntax : MemberDeclarationSyntax, ITypeSymbol
    {
        public FixedList<MemberDeclarationSyntax> Members { get; }

        public TypePromise DeclaresType { get; } = new TypePromise();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ITypeSymbol.DeclaresType => DeclaresType.Fulfilled();

        protected TypeDeclarationSyntax(
            CodeFile file,
            TextSpan nameSpan,
            Name fullName,
            FixedList<MemberDeclarationSyntax> members)
            : base(file, fullName, nameSpan, new SymbolSet(members))
        {
            Members = members;
            foreach (var member in Members)
                member.DeclaringType = this;
        }
    }
}
