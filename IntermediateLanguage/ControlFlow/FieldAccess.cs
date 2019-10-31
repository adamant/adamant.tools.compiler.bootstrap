using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class FieldAccess : Value, IPlace
    {
        public IPlace Expression { get; }
        public Name Field { get; }

        public FieldAccess(IPlace expression, Name field, TextSpan span)
            : base(span)
        {
            Expression = expression;
            Field = field;
        }

        public Variable CoreVariable()
        {
            return Expression.CoreVariable();
        }

        public override string ToString()
        {
            return $"({Expression}).{Field.UnqualifiedName}";
        }
    }
}
