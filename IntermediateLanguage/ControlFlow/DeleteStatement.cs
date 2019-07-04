using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class DeleteStatement : ExpressionStatement
    {
        public IPlace Place { get; }
        public UserObjectType Type { get; }

        public DeleteStatement(IPlace place, UserObjectType type, TextSpan span, Scope scope)
            : base(span, scope)
        {
            Place = place;
            Type = type;
        }

        public override Statement Clone()
        {
            return new DeleteStatement(Place, Type, Span, Scope);
        }

        // Useful for debugging
        public override string ToStatementString()
        {
            return $"delete {Place};";
        }
    }
}
