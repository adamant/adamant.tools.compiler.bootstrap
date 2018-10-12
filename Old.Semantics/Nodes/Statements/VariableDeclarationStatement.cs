using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Statements
{
    public class VariableDeclarationStatement : Statement
    {
        public new VariableDeclarationStatementSyntax Syntax { get; }
        public bool MutableBinding { get; }
        public string Name { get; }
        public DataType Type { get; }
        public Expression Initializer { get; }

        public VariableDeclarationStatement(
            VariableDeclarationStatementSyntax syntax,
            IEnumerable<Diagnostic> diagnostics,
            bool mutableBinding,
            string name,
            DataType type,
            Expression initializer)
            : base(diagnostics)
        {
            Syntax = syntax;
            MutableBinding = mutableBinding;
            Name = name;
            Type = type;
            Initializer = initializer;
        }

        protected override SyntaxNode GetSyntax() => Syntax;

        public override void AllDiagnostics(List<Diagnostic> list)
        {
            base.AllDiagnostics(list);
            Initializer?.AllDiagnostics(list);
        }
    }
}