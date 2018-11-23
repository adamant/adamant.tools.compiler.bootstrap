using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class ParameterSyntax : Syntax, ISymbol
    {
        public TextSpan Span { get; }
        public bool MutableBinding { get; }
        [NotNull] public SimpleName Name { get; }

        [NotNull] public TypePromise Type { get; } = new TypePromise();

        public Name FullName => throw new System.NotImplementedException();

        DataType ISymbol.Type => Type.Resolved();

        public ISymbol ComposeWith(ISymbol symbol)
        {
            throw new System.NotImplementedException();
        }

        public ISymbol Lookup(SimpleName name)
        {
            throw new System.NotImplementedException();
        }

        protected ParameterSyntax(TextSpan span, bool mutableBinding, [NotNull] SimpleName name)
        {
            Span = span;
            MutableBinding = mutableBinding;
            Name = name;
        }
    }
}
