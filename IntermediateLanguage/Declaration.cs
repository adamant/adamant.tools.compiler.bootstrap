using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    [Closed(
        typeof(TypeDeclaration),
        typeof(FunctionDeclaration),
        typeof(FieldDeclaration),
        typeof(ConstructorDeclaration))]
    public abstract class Declaration : ISymbol
    {
        // Declarations are not mutable bindings
        bool ISymbol.MutableBinding => false;
        public bool IsMember { get; }
        public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public DataType Type { get; }

        public DataType DeclaresType { get; }

        public SymbolSet ChildSymbols { get; }

        protected Declaration(
            bool isMember,
            Name fullName,
            DataType type,
            SymbolSet childSymbols)
        {
            IsMember = isMember;
            FullName = fullName;
            Type = type;
            ChildSymbols = childSymbols;
        }

        public override string ToString()
        {
            return FullName.ToString();
        }
    }
}
