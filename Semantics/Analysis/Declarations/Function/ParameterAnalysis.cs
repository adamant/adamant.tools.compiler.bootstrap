using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations.Function
{
    public class ParameterAnalysis : AnalysisNode, IDeclarationAnalysis
    {
        [NotNull] public new ParameterSyntax Syntax { get; }
        public bool MutableBinding { get; }
        [NotNull] public QualifiedName Name { get; }
        // TypeExpression can be null for self parameters
        [CanBeNull] public ExpressionAnalysis TypeExpression { get; }
        [CanBeNull] public DataType Type { get; set; }

        public ParameterAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ParameterSyntax syntax,
            [NotNull] QualifiedName name,
            [CanBeNull] ExpressionAnalysis typeExpression)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            Name = name;
            TypeExpression = typeExpression;
            MutableBinding = syntax is NamedParameterSyntax np && np.VarKeyword != null;
        }

        [NotNull]
        public Parameter Complete()
        {
            return new Parameter(MutableBinding, Name, Type.AssertNotNull());
        }
    }
}
