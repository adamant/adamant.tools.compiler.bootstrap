using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class FunctionAnalysis : DeclarationAnalysis
    {
        [NotNull] public FunctionDeclarationSyntax Syntax { get; }
        [NotNull] public new FunctionDeclaration Semantics { get; }

        public FunctionAnalysis(
            [NotNull] CodeFile file,
            [NotNull] LexicalScope scope,
            [NotNull] FunctionDeclarationSyntax syntax,
            [NotNull] FunctionDeclaration semantics)
            : base(file, scope, semantics)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(semantics), semantics);
            Syntax = syntax;
            Semantics = semantics;
        }
    }
}
