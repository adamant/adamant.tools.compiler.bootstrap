using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class GenericParameterSyntax : Syntax
    {
        public bool IsParams { get; }
        [NotNull] public SimpleName Name { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        public GenericParameterSyntax(
            bool isParams,
            [NotNull] string name,
            [CanBeNull] ExpressionSyntax typeExpression)
        {
            Name = new SimpleName(name);
            TypeExpression = typeExpression;
            IsParams = isParams;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
