using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class GenericParameterAnalysis : AnalysisNode, IDeclarationAnalysis
    {
        [NotNull] public new GenericParameterSyntax Syntax { get; }
        [NotNull] public QualifiedName Name { get; }
        // TypeExpression can be null for self parameters
        [CanBeNull] public ExpressionAnalysis TypeExpression { get; }
        [CanBeNull] public DataType Type { get; set; }

        public GenericParameterAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] GenericParameterSyntax syntax,
            [NotNull] QualifiedName name,
            [CanBeNull] ExpressionAnalysis typeExpression)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            Name = name;
            TypeExpression = typeExpression;
        }
    }
}
