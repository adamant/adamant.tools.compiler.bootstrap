using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG
{
    public class VariableDeclaration
    {
        /// <summary>
        /// Which IL variable is being declared
        /// </summary>
        public Variable Variable { get; }

        /// <summary>
        /// What scope is this variable in. The %result or self parameter of a
        /// constructor don't have scopes.
        /// </summary>
        public Scope? Scope { get; }

        /// <summary>
        /// If this declaration corresponds to an argument or local variable,
        /// The symbol. (All temp variables don't have a symbol)
        /// </summary>
        public BindingSymbol? Symbol { get; }
        public bool IsParameter { get; }
        public bool IsMutableBinding { get; }
        public DataType Type { get; }
        public bool TypeIsNotEmpty => !Type.IsEmpty;

        public VariableDeclaration(
            bool isParameter,
            bool isMutableBinding,
            DataType type,
            Variable variable,
            Scope? scope,
            BindingSymbol? symbol = null)
        {
            IsParameter = isParameter;
            Variable = variable;
            Scope = scope;
            Symbol = symbol;
            IsMutableBinding = isMutableBinding;
            Type = type;
        }

        public VariableReference Reference(TextSpan span)
        {
            return new VariableReference(Variable, span);
        }

        //public VariableReference Move(TextSpan span)
        //{
        //    return new VariableReference(Variable, span);
        //}

        public VariablePlace Place(TextSpan span)
        {
            return new VariablePlace(Variable, span);
        }

        // Useful for debugging
        public override string ToString()
        {
            return ToStatementString() + ContextCommentString();
        }

        public string ToStatementString()
        {
            var binding = IsMutableBinding ? "var" : "let";
            var name = Symbol is null ? "" : $"({Symbol})";
            return $"{binding} {Variable}{name}: {Type};";
        }

        public string ContextCommentString()
        {
            return Scope != null ? $" // in {Scope}" : "";
        }
    }
}
