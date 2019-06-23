using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class VariableDeclaration
    {
        /// <summary>
        /// Which IL variable is being declared
        /// </summary>
        public readonly Variable Variable;

        /// <summary>
        /// What scope is this variable in. The %result or self parameter of a
        /// constructor don't have scopes.
        /// </summary>
        public readonly Scope? Scope;

        /// <summary>
        /// If this declaration corresponds to an argument or local variable,
        /// what it was named. Not guaranteed unique
        /// </summary>
        public readonly SimpleName Name;
        public readonly bool IsParameter;
        public readonly bool IsMutableBinding;
        public readonly DataType Type;
        public bool TypeIsNotEmpty => !Type.IsEmpty;

        // TODO does this make sense? shouldn't the default reference kind always be Share?
        private readonly ValueSemantics defaultSemantics;

        public VariableDeclaration(
            bool isParameter,
            bool isMutableBinding,
            DataType type,
            Variable variable,
            Scope? scope,
            SimpleName name = null)
        {
            IsParameter = isParameter;
            Variable = variable;
            Scope = scope;
            Name = name;
            IsMutableBinding = isMutableBinding;
            Type = type;
            if (type.ValueSemantics == ValueSemantics.Own)
                if (type is ReferenceType referenceType && referenceType.Mutability == Mutability.Immutable)
                    defaultSemantics = ValueSemantics.Alias;
                else
                    defaultSemantics = ValueSemantics.Borrow;
            else
                defaultSemantics = type.ValueSemantics;
        }

        public VariableReference Reference(TextSpan span)
        {
            return new VariableReference(Variable, defaultSemantics, span);
        }

        public VariableReference LValueReference(TextSpan span)
        {
            return new VariableReference(Variable, ValueSemantics.LValue, span);
        }

        // Useful for debugging
        public override string ToString()
        {
            var binding = IsMutableBinding ? "var" : "let";
            var name = Name != null ? $"({Name})" : "";
            var scope = Scope != null ? $" // in {Scope}" : "";
            return $"{binding} {Variable}{name}: {Type};{scope}";
        }
    }
}
