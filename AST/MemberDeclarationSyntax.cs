using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(FunctionDeclarationSyntax),
        typeof(FieldDeclarationSyntax),
        //typeof(ConstDeclarationSyntax),
        typeof(TypeDeclarationSyntax))]
    public abstract class MemberDeclarationSyntax : DeclarationSyntax, ISymbol
    {
        // Declarations are not mutable bindings
        bool ISymbol.MutableBinding => false;

        public TypeDeclarationSyntax DeclaringType { get; internal set; }

        public Name FullName { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public TypePromise Type { get; } = new TypePromise();

        DataType ISymbol.DeclaresType => null;

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ISymbol.Type => Type.Fulfilled();

        public SymbolSet ChildSymbols { get; protected set; }

        protected MemberDeclarationSyntax(
            CodeFile file,
            Name fullName,
            TextSpan nameSpan,
            SymbolSet childSymbols = null)
            : base(file, nameSpan)
        {
            FullName = fullName;
            ChildSymbols = childSymbols ?? SymbolSet.Empty;
        }
    }
}
