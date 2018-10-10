using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions
{
    public class VariableExpression : Expression
    {
        public new IdentifierNameSyntax Syntax { get; }
        public string Name => Syntax.Name.Value;
        public VariableName VariableName { get; }

        public VariableExpression(
            IdentifierNameSyntax syntax,
            IEnumerable<Diagnostic> diagnostics,
            VariableName variableName,
            DataType type)
            : base(diagnostics, type)
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
