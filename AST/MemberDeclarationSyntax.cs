using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(FunctionDeclarationSyntax),
        typeof(FieldDeclarationSyntax))]
    public abstract class MemberDeclarationSyntax : DeclarationSyntax, ISymbol
    {
        public ClassDeclarationSyntax? DeclaringType { get; internal set; }

        public Name FullName { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public SymbolSet ChildSymbols { get; protected set; }

        protected MemberDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            Name fullName,
            TextSpan nameSpan,
            SymbolSet? childSymbols = null)
            : base(span, file, nameSpan)
        {
            FullName = fullName;
            ChildSymbols = childSymbols ?? SymbolSet.Empty;
        }
    }
}
