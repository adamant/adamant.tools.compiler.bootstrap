using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class TypeName : SemanticNode
    {
        public new TypeSyntax Syntax { get; }
        public DataType Type { get; }

        public TypeName(TypeSyntax syntax, IEnumerable<DiagnosticInfo> diagnostics, DataType type)
            : base(diagnostics)
        {
            Syntax = syntax;
            Type = type;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
