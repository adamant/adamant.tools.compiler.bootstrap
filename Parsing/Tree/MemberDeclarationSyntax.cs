using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class MemberDeclarationSyntax : DeclarationSyntax, IMemberDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringType { get; }

        public Name FullName { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public SymbolSet ChildSymbols { get; protected set; }

        protected MemberDeclarationSyntax(
            IClassDeclarationSyntax declaringType,
            TextSpan span,
            CodeFile file,
            Name fullName,
            TextSpan nameSpan,
            SymbolSet? childSymbols = null)
            : base(span, file, nameSpan)
        {
            DeclaringType = declaringType;
            FullName = fullName;
            ChildSymbols = childSymbols ?? SymbolSet.Empty;
        }
    }
}
