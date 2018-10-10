using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
        [NotNull] public new FunctionDeclarationSyntax Syntax { get; }
        [NotNull] public AccessModifier Access { get; }
        [NotNull] public string Name { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<Parameter> Parameters { get; }
        [NotNull] public Expression ReturnType { get; }
        [NotNull] public Block Body { get; }

        public FunctionDeclaration(
            [NotNull] FunctionDeclarationSyntax syntax,
            [NotNull] [ItemNotNull] IEnumerable<Diagnostic> diagnostics,
            [NotNull] AccessModifier access,
            [NotNull] string name,
            [NotNull] [ItemNotNull] IEnumerable<Parameter> parameters,
            [NotNull] Expression returnType,
            [NotNull] Block body)
            : base(diagnostics)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(access), access);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(returnType), returnType);
            Requires.NotNull(nameof(body), body);
            Syntax = syntax;
            Access = access;
            Name = name;
            Parameters = parameters.ToList().AsReadOnly().AssertNotNull();
            ReturnType = returnType;
            Body = body;
        }

        [NotNull]
        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }

        public override void AllDiagnostics([NotNull] List<Diagnostic> list)
        {
            Requires.NotNull(nameof(list), list);
            base.AllDiagnostics(list);
            foreach (var parameter in Parameters)
                parameter.AllDiagnostics(list);
            ReturnType.AllDiagnostics(list);
            Body.AllDiagnostics(list);
        }
    }
}
