using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class VariableDeclarationStatementAnalysis : StatementAnalysis, IDeclarationAnalysis
    {
        [NotNull] public new VariableDeclarationStatementSyntax Syntax { get; }
        public bool MutableBinding => Syntax.Binding is IVarKeywordToken;
        [NotNull] public Name Name { get; }
        [CanBeNull] public ExpressionAnalysis TypeExpression { get; }
        [CanBeNull] public ExpressionAnalysis Initializer { get; }
        [NotNull] public TypeAnalysis Type { get; } = new TypeAnalysis();

        public VariableDeclarationStatementAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] VariableDeclarationStatementSyntax syntax,
            [NotNull] Name name,
            [CanBeNull] ExpressionAnalysis typeExpression,
            [CanBeNull] ExpressionAnalysis initializer)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(name), name);
            Syntax = syntax;
            Name = name;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }

        IEnumerable<DataType> ISymbol.Types => Type.DataType.YieldValue();
        ISymbol ISymbol.ComposeWith(ISymbol symbol)
        {
            if (symbol is CompositeSymbol composite)
                return composite.ComposeWith(this);
            return new CompositeSymbol(this, symbol);
        }
    }
}
