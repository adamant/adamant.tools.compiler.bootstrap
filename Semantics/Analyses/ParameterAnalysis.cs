using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class ParameterAnalysis : SyntaxAnalysis, IDeclarationAnalysis
    {
        [NotNull] public new ParameterSyntax Syntax { get; }
        public bool MutableBinding { get; }
        [NotNull] public Name Name { get; }

        // TypeExpression can be null for self parameters
        [CanBeNull] public ExpressionAnalysis TypeExpression { get; }
        [NotNull] public TypeAnalysis Type { get; } = new TypeAnalysis();

        public ParameterAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ParameterSyntax syntax,
            [NotNull] Name name,
            [CanBeNull] ExpressionAnalysis typeExpression)
            : base(context)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            Name = name;
            TypeExpression = typeExpression;
            MutableBinding = syntax is NamedParameterSyntax np && np.MutableBinding;
        }

        IEnumerable<DataType> ISymbol.Types => throw new System.NotImplementedException();

        ISymbol ISymbol.ComposeWith(ISymbol symbol)
        {
            throw new System.NotImplementedException();
        }

        [CanBeNull]
        public ISymbol Lookup([NotNull] SimpleName name)
        {
            throw new System.NotImplementedException();
        }

        [NotNull]
        public Parameter Complete()
        {
            return new Parameter(MutableBinding, Name, Type.AssertResolved());
        }
    }
}
