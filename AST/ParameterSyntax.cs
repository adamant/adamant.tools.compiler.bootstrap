using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class ParameterSyntax : Syntax, ISymbol
    {
        public TextSpan Span { get; }
        public bool MutableBinding { get; }
        [NotNull] public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;
        public bool Unused { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public SimpleName LookupByName => FullName.UnqualifiedName.WithoutNumber();
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ISymbol.Type => Type.Fulfilled();
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        SymbolSet ISymbol.ChildSymbols => SymbolSet.Empty;

        protected ParameterSyntax(TextSpan span, bool mutableBinding, [NotNull] Name fullName)
        {
            Span = span;
            MutableBinding = mutableBinding;
            FullName = fullName;
            Unused = fullName.UnqualifiedName.Text.StartsWith("_");
        }
    }
}
