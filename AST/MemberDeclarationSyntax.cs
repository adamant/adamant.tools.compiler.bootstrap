using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class MemberDeclarationSyntax : DeclarationSyntax, ISymbol
    {
        [CanBeNull] public TypeDeclarationSyntax DeclaringType { get; set; }

        [NotNull] public Name FullName { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        public SimpleName Name => FullName.UnqualifiedName;

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        public virtual SimpleName LookupByName => FullName.UnqualifiedName;

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        DataType ISymbol.Type => GetDataType();

        public SymbolSet ChildSymbols { get; }

        protected MemberDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] Name fullName,
            TextSpan nameSpan,
            [CanBeNull] SymbolSet childSymbols = null)
            : base(file, nameSpan)
        {
            FullName = fullName;
            ChildSymbols = childSymbols ?? SymbolSet.Empty;
        }

        [NotNull] protected abstract DataType GetDataType();
    }
}
