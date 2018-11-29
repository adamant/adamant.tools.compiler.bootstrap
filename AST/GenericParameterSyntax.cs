using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class GenericParameterSyntax : Syntax
    {
        public bool IsLifetime { get; }
        public bool IsParams { get; }
        [NotNull] public Name FullName { get; }
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        public GenericParameterSyntax(
            bool isLifetime,
            bool isParams,
            [NotNull] Name fullName,
            [CanBeNull] ExpressionSyntax typeExpression)
        {
            FullName = fullName;
            TypeExpression = typeExpression;
            IsLifetime = isLifetime;
            IsParams = isParams;
        }

        public override string ToString()
        {
            if (IsLifetime) return "$" + Name;
            var paramsKeyword = IsParams ? "params " : "";
            if (TypeExpression != null) return $"{paramsKeyword}{Name} : {TypeExpression}";
            return $"{paramsKeyword}{Name}";
        }
    }
}
