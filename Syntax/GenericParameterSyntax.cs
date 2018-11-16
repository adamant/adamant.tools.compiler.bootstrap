using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericParameterSyntax : NonTerminal
    {
        public bool IsParams { get; }
        [NotNull] public SimpleName Name { get; }
        [CanBeNull] public ExpressionSyntax Type { get; }

        public GenericParameterSyntax(
            bool isParams,
            [NotNull] string name,
            [CanBeNull] ExpressionSyntax type)
        {
            Name = new SimpleName(name);
            Type = type;
            IsParams = isParams;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
