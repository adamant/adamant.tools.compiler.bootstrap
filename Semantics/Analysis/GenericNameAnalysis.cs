using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class GenericNameAnalysis : SimpleNameAnalysis
    {
        [NotNull] public new GenericNameSyntax Syntax { get; }
        [CanBeNull] public DataType NameType { get; set; }
        [NotNull] [ItemNotNull] public IReadOnlyList<ArgumentAnalysis> Arguments { get; }
        public int GenericArity => Arguments.Count;

        public GenericNameAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] GenericNameSyntax syntax,
            [NotNull, ItemNotNull] IEnumerable<ArgumentAnalysis> arguments)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(arguments), arguments);
            Syntax = syntax;
            Arguments = arguments.ToReadOnlyList();
        }
    }
}
