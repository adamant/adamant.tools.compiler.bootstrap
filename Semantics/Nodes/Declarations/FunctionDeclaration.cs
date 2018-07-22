using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations
{
    public class FunctionDeclaration : Declaration
    {
        public new FunctionDeclarationSyntax Syntax { get; }
        public AccessLevel Access { get; }
        public string Name { get; }
        public DataType ReturnType { get; }
        public Block Body { get; }
        public IReadOnlyList<Parameter> Parameters { get; }

        public FunctionDeclaration(
            FunctionDeclarationSyntax syntax,
            AccessLevel access,
            string name,
            IEnumerable<Parameter> parameters,
            DataType returnType,
            Block body)
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
