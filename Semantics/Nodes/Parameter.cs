using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class Parameter : SemanticNode
    {
        public new ParameterSyntax Syntax { get; }
        public bool MutableBinding { get; }
        public string Name { get; }
        public TypeName Type { get; }

        public Parameter(
            ParameterSyntax syntax,
            IEnumerable<DiagnosticInfo> diagnostics,
            bool mutableBinding,
            string name,
            TypeName type)
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
    }
}
