using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class AnalysisContext
    {
        [NotNull] public CodeFile File { get; }
        [NotNull] public LexicalScope Scope { get; }

        public AnalysisContext([NotNull] CodeFile file, [NotNull] LexicalScope scope)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(scope), scope);
            File = file;
            Scope = scope;
        }

        [NotNull]
        public AnalysisContext InFunction([NotNull] NamedFunctionDeclarationSyntax syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            return new AnalysisContext(File, new FunctionScope(Scope, syntax));
        }

        [NotNull]
        public AnalysisContext InBlock([NotNull] BlockExpressionSyntax syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            return new AnalysisContext(File, new BlockScope(Scope, syntax));
        }
    }
}
