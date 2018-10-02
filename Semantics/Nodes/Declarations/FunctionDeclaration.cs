using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations
{
    public class FunctionDeclaration : MemberDeclaration
    {
        public new FunctionDeclarationSyntax Syntax { get; }
        public AccessLevel Access { get; }
        public string Name { get; }
        public IReadOnlyList<Parameter> Parameters { get; }
        public Expression ReturnType { get; }
        public Block Body { get; }

        public FunctionDeclaration(
            FunctionDeclarationSyntax syntax,
            IEnumerable<DiagnosticInfo> diagnostics,
            AccessLevel access,
            string name,
            IEnumerable<Parameter> parameters,
            Expression returnType,
            Block body)
            : base(diagnostics)
        {
            Syntax = syntax;
            Access = access;
            Name = name;
            Parameters = parameters.ToList().AsReadOnly();
            ReturnType = returnType;
            Body = body;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
