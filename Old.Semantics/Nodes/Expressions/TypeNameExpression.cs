using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions
{
    public class TypeNameExpression : Expression
    {
        public new TypeSyntax Syntax { get; }

        public TypeNameExpression(TypeSyntax syntax, IEnumerable<Diagnostic> diagnostics, DataType type)
            : base(diagnostics, type)
        {
            Syntax = syntax;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
