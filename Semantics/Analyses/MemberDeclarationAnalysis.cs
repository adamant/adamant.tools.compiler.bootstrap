using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public abstract class MemberDeclarationAnalysis : DeclarationAnalysis, IDeclarationAnalysis
    {
        [NotNull] public new MemberDeclarationSyntax Syntax { get; }
        [NotNull] public Diagnostics Diagnostics { get; }
        [NotNull] public Name Name { get; }
        [CanBeNull, ItemNotNull] public FixedList<GenericParameterAnalysis> GenericParameters { get; }
        public bool IsGeneric => GenericParameters != null;
        public int? GenericArity => GenericParameters?.Count;
        // This is the type of the value provided by using the name. So for
        // classes, it is the metatype, for functions, the function type
        [NotNull] public TypeAnalysis Type { get; } = new TypeAnalysis();

        protected MemberDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] MemberDeclarationSyntax syntax,
            [NotNull] Name name,
            [CanBeNull, ItemNotNull] IEnumerable<GenericParameterAnalysis> genericParameters = null)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(name), name);
            Syntax = syntax;
            Diagnostics = new Diagnostics();
            Name = name;
            GenericParameters = genericParameters?.ToFixedList();
        }

        IEnumerable<DataType> ISymbol.Types => Type.DataType.YieldValue();
        ISymbol ISymbol.ComposeWith(ISymbol symbol)
        {
            if (symbol is CompositeSymbol composite)
                return composite.ComposeWith(this);
            return new CompositeSymbol(this, symbol);
        }

        [CanBeNull]
        public abstract Declaration Complete([NotNull] Diagnostics diagnostics);

        protected bool CompleteDiagnostics([NotNull] Diagnostics diagnostics)
        {
            var myDiagnostics = Diagnostics.Build();
            var errors = myDiagnostics.Any(d => d.Level >= DiagnosticLevel.CompilationError);
            diagnostics.Add(myDiagnostics);
            return errors;
        }
    }
}
