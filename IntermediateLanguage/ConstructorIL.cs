using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ConstructorIL : DeclarationIL, IInvocableDeclarationIL
    {
        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "NA")]
        bool IInvocableDeclarationIL.IsConstructor => true;
        public FixedList<ParameterIL> Parameters { get; }
        public int Arity => Parameters.Count;
        public FixedList<FieldInitializationIL> FieldInitializations { get; }
        public ControlFlowGraph IL { get; }
        public new ConstructorSymbol Symbol { get; }

        public ConstructorIL(
            ConstructorSymbol symbol,
            FixedList<ParameterIL> parameters,
            FixedList<FieldInitializationIL> fieldInitializations,
            ControlFlowGraph il)
            : base(true, symbol)
        {
            IL = il;
            FieldInitializations = fieldInitializations;
            Symbol = symbol;
            Parameters = parameters;
        }
    }
}
