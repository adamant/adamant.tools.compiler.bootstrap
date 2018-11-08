using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class ForeachExpressionAnalysis : ExpressionAnalysis, IDeclarationAnalysis, ILocalVariableScopeAnalysis
    {
        [NotNull] public new ForeachExpressionSyntax Syntax { get; }
        [NotNull] public Name VariableName { get; }
        [CanBeNull] public ExpressionAnalysis TypeExpression { get; set; }
        [CanBeNull] public TypeAnalysis VariableType { get; set; }
        [NotNull] public ExpressionAnalysis InExpression { get; }
        [NotNull] public BlockAnalysis Block { get; }

        public ForeachExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ForeachExpressionSyntax syntax,
            [NotNull] Name variableName,
            [CanBeNull] ExpressionAnalysis typeExpression,
            [NotNull] ExpressionAnalysis inExpression,
            [NotNull] BlockAnalysis block)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(variableName), variableName);
            Requires.NotNull(nameof(inExpression), inExpression);
            Requires.NotNull(nameof(block), block);
            Syntax = syntax;
            Block = block;
            TypeExpression = typeExpression;
            VariableName = variableName;
            InExpression = inExpression;
        }

        IEnumerable<IDeclarationAnalysis> ILocalVariableScopeAnalysis.LocalVariableDeclarations()
        {
            yield return this;
        }

        Name ISymbol.Name => VariableName;
        IEnumerable<DataType> ISymbol.Types => VariableType.DataType.YieldValue();
        ISymbol ISymbol.ComposeWith(ISymbol symbol)
        {
            if (symbol is CompositeSymbol composite)
                return composite.ComposeWith(this);
            return new CompositeSymbol(this, symbol);
        }
    }
}
