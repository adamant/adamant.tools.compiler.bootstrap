using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations
{
    public class FunctionDeclaration : MemberDeclaration
    {
        public new FunctionDeclarationSyntax Syntax { get; }
        public AccessModifier Access { get; }
        [NotNull] public string Name { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<Parameter> Parameters { get; }
        [NotNull] public Expression ReturnType { get; }
        public Block Body { get; }

        public FunctionDeclaration(
            FunctionDeclarationSyntax syntax,
            IEnumerable<Diagnostic> diagnostics,
            AccessModifier access,
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

        public override void AllDiagnostics(List<Diagnostic> list)
        {
            base.AllDiagnostics(list);
            foreach (var parameter in Parameters)
                parameter.AllDiagnostics(list);
            ReturnType.AllDiagnostics(list);
            Body.AllDiagnostics(list);
        }
    }
}
