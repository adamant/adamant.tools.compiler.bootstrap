using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class ParameterSyntax : Syntax
    {
        public TextSpan Span { get; }
        public bool MutableBinding { get; }
        [NotNull] public SimpleName Name { get; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        protected ParameterSyntax(TextSpan span, bool mutableBinding, [NotNull] SimpleName name)
        {
            Span = span;
            MutableBinding = mutableBinding;
            Name = name;
        }
    }
}
