using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        DataType ISymbol.Type => Type.Resolved();
        FixedDictionary<SimpleName, ISymbol> ISymbol.ChildSymbols => FixedDictionary<SimpleName, ISymbol>.Empty;

        protected ParameterSyntax(TextSpan span, bool mutableBinding, [NotNull] Name fullName)
        {
            Span = span;
            MutableBinding = mutableBinding;
            FullName = fullName;
        }
    }
}
