using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types.Names
{
    public class GenericNameAnalysis : SimpleNameAnalysis
    {
        [NotNull] public new GenericNameSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<ArgumentAnalysis> Arguments { get; }

        public GenericNameAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] GenericNameSyntax syntax,
            [NotNull][ItemNotNull] IEnumerable<ArgumentAnalysis> arguments)
            : base(context, syntax)
        {
            Syntax = syntax;
            Requires.NotNull(nameof(arguments), arguments);
            Arguments = arguments.ToReadOnlyList();
        }
    }
}
