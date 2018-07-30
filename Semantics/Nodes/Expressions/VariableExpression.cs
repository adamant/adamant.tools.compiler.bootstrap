using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions
{
    public class VariableExpression : Expression
    {
        public new IdentifierNameSyntax Syntax { get; }
        public string Name => Syntax.Identifier.Text;
        public VariableName VariableName { get; }

        public VariableExpression(
            IdentifierNameSyntax syntax,
            VariableName variableName,
            DataType type)
            : base(type)
        {
            Syntax = syntax;
            VariableName = variableName;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
