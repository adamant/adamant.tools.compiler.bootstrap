using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public abstract class MemberDeclarationAnalysis : DeclarationAnalysis, IDeclarationAnalysis
    {
        [NotNull] public new MemberDeclarationSyntax Syntax { get; }
        [NotNull] public DiagnosticsBuilder Diagnostics { get; }
        [NotNull] public Name Name { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<GenericParameterAnalysis> GenericParameters { get; }
        public int GenericArity => GenericParameters.Count;
        // This is the type of the value provided by using the name. So for
        // classes, it is the metatype, for functions, the function type
        [CanBeNull] public DataType Type => GetDataType();

        protected MemberDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] MemberDeclarationSyntax syntax,
            [NotNull] Name name,
            [NotNull] [ItemNotNull] IEnumerable<GenericParameterAnalysis> genericParameters)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(name), name);
            Syntax = syntax;
            Diagnostics = new DiagnosticsBuilder();
            Name = name;
            GenericParameters = genericParameters.ToReadOnlyList();
        }

        [CanBeNull]
        protected abstract DataType GetDataType();

        IEnumerable<DataType> ISymbol.Types => Type.YieldValue();
        ISymbol ISymbol.ComposeWith(ISymbol symbol)
        {
            if (symbol is CompositeSymbol composite)
                return composite.ComposeWith(this);
            return new CompositeSymbol(this, symbol);
        }

        [CanBeNull]
        public abstract Declaration Complete([NotNull] DiagnosticsBuilder diagnostics);

        protected bool CompleteDiagnostics([NotNull] DiagnosticsBuilder diagnostics)
        {
            var errors = Diagnostics.Any(d => d.Level >= DiagnosticLevel.CompilationError);
            diagnostics.Publish(Diagnostics.Build());
            return errors;
        }
    }
}
