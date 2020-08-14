using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ConstructorDeclaration : Declaration, IInvocableDeclaration
    {
        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "NA")]
        bool IInvocableDeclaration.IsConstructor => true;
        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public FixedList<FieldInitialization> FieldInitializations { get; }
        public ControlFlowGraph IL { get; }
        public new ConstructorSymbol Symbol { get; }

        public ConstructorDeclaration(
            MaybeQualifiedName fullName,
            FixedList<Parameter> parameters,
            FixedList<FieldInitialization> fieldInitializations,
            ConstructorSymbol symbol,
            ControlFlowGraph il)
            : base(true, fullName, symbol)
        {
            IL = il;
            FieldInitializations = fieldInitializations;
            Symbol = symbol;
            Parameters = parameters;
        }
    }
}
