using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions
{
    public class ExpressionAnalysis
    {
        [NotNull] public CodeFile File { get; }
        [NotNull] public LexicalScope Scope { get; }
        [NotNull] public ExpressionSyntax Syntax { get; }
        [CanBeNull] public DataType Type { get; internal set; }

        public ExpressionAnalysis(
            [NotNull] CodeFile file,
            [NotNull] LexicalScope scope,
            [NotNull] ExpressionSyntax syntax)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(scope), scope);
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            File = file;
            Scope = scope;
        }
    }
}
