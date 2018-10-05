using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class Parameter : SemanticNode
    {
        public new ParameterSyntax Syntax { get; }
        public bool MutableBinding { get; }
        public string Name { get; }
        public Expression Type { get; }

        public Parameter(
            ParameterSyntax syntax,
            IEnumerable<Diagnostic> diagnostics,
            bool mutableBinding,
            string name,
            Expression type)
            : base(diagnostics)
        {
            Syntax = syntax;
            MutableBinding = mutableBinding;
            Name = name;
            Type = type;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }

        public override void AllDiagnostics(List<Diagnostic> list)
        {
            base.AllDiagnostics(list);
            Type.AllDiagnostics(list);
        }
    }
}
