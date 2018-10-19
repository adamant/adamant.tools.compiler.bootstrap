using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public class ParameterAnalysis
    {
        [NotNull] public ParameterSyntax Syntax { get; }
        public bool MutableBinding { get; }
        [NotNull] public string Name { get; }
        [NotNull] public ExpressionAnalysis TypeExpression { get; }
        [CanBeNull] public DataType Type { get; set; }

        public ParameterAnalysis(
            [NotNull] ParameterSyntax syntax,
            [NotNull] ExpressionAnalysis typeExpression)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(typeExpression), typeExpression);
            Syntax = syntax;
            TypeExpression = typeExpression;
            MutableBinding = syntax.VarKeyword != null;
            Name = syntax.Name.Value ?? "_";
        }

        [NotNull]
        public Parameter Complete()
        {
            return new Parameter(MutableBinding, Name, Type.AssertNotNull());
        }
    }
}
