using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.TypeChecking;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class GenericParameterAnalysis : SyntaxAnalysis, IDeclarationAnalysis
    {
        [NotNull] public new GenericParameterSyntax Syntax { get; }
        [NotNull] public Name Name { get; }
        // TypeExpression can be null for self parameters
        [CanBeNull] public ExpressionAnalysis TypeExpression { get; }
        [CanBeNull] public TypeAnalysis Type { get; set; }

        public GenericParameterAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] GenericParameterSyntax syntax,
            [NotNull] Name name,
            [CanBeNull] ExpressionAnalysis typeExpression)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            Name = name;
            TypeExpression = typeExpression;
        }

        IEnumerable<DataType> ISymbol.Types => Type.DataType.YieldValue();
        ISymbol ISymbol.ComposeWith(ISymbol symbol)
        {
            if (symbol is CompositeSymbol composite)
                return composite.ComposeWith(this);
            return new CompositeSymbol(this, symbol);
        }

        [NotNull]
        public GenericParameter Complete()
        {
            return new GenericParameter(Name, Type.AssertResolved());
        }
    }
}
