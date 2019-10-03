using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    [Closed(
        typeof(ClassDeclaration),
        typeof(FunctionDeclaration),
        typeof(FieldDeclaration),
        typeof(ConstructorDeclaration))]
    public abstract class Declaration : ISymbol
    {
        public bool IsMember { get; }
        public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public SymbolSet ChildSymbols { get; }

        protected Declaration(bool isMember, Name fullName, SymbolSet childSymbols)
        {
            IsMember = isMember;
            FullName = fullName;
            ChildSymbols = childSymbols;
        }

        public override string ToString()
        {
            return FullName.ToString();
        }
    }
}
