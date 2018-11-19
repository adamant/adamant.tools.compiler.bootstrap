using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class VariableDeclarationStatementAnalysis : StatementAnalysis, IDeclarationAnalysis
    {
        [NotNull] public new VariableDeclarationStatementSyntax Syntax { get; }
        public bool MutableBinding => Syntax.MutableBinding;
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

        [NotNull]
        ISymbol ISymbol.ComposeWith([NotNull] ISymbol symbol)
        {
            if (symbol is CompositeSymbol composite)
                return composite.ComposeWith(this);
            return new CompositeSymbol(this, symbol);
        }

        [CanBeNull]
        public ISymbol Lookup([NotNull] SimpleName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
