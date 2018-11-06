using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class VariableDeclarationStatementAnalysis : StatementAnalysis, IDeclarationAnalysis
    {
        [NotNull] public new VariableDeclarationStatementSyntax Syntax { get; }
        public bool MutableBinding => Syntax.Binding is VarKeywordToken;
        [NotNull] public QualifiedName Name { get; }
        [CanBeNull] public ExpressionAnalysis TypeExpression { get; }
        [CanBeNull] public ExpressionAnalysis Initializer { get; }
        [CanBeNull] public DataType Type { get; set; }

        public VariableDeclarationStatementAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] VariableDeclarationStatementSyntax syntax,
            [NotNull] QualifiedName name,
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

        IEnumerable<DataType> ISymbol.Types => Type.YieldValue();
        ISymbol ISymbol.ComposeWith(ISymbol symbol)
        {
            if (symbol is CompositeSymbol composite)
                return composite.ComposeWith(this);
            return new CompositeSymbol(this, symbol);
        }
    }
}
