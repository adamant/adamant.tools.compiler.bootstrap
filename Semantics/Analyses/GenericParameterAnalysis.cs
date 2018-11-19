using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class GenericParameterAnalysis : SyntaxAnalysis, IDeclarationAnalysis
    {
        [NotNull] public new GenericParameterSyntax Syntax { get; }
        [NotNull] public Name Name { get; }
        // TypeExpression can be null for self parameters
        [CanBeNull] public ExpressionAnalysis TypeExpression { get; }
        [NotNull] public TypeAnalysis Type { get; } = new TypeAnalysis();

        public GenericParameterAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] GenericParameterSyntax syntax,
            [NotNull] Name name,
            [CanBeNull] ExpressionAnalysis typeExpression)
            : base(context)
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

        [CanBeNull]
        public ISymbol Lookup([NotNull] SimpleName name)
        {
            throw new System.NotImplementedException();
        }

        [NotNull]
        public GenericParameter Complete()
        {
            return new GenericParameter(Name, Type.AssertResolved());
        }
    }
}
