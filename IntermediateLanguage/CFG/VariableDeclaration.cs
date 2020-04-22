using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

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
        /// what it was named. Not guaranteed unique.
        /// </summary>
        public SimpleName? Name { get; }
        public bool IsParameter { get; }
        public bool IsMutableBinding { get; }
        public DataType Type { get; }
        public bool TypeIsNotEmpty => !Type.IsEmpty;

        // TODO does this make sense? shouldn't the default reference kind always be Share?
        private readonly ValueSemantics defaultSemantics;

        public VariableDeclaration(
            bool isParameter,
            bool isMutableBinding,
            DataType type,
            Variable variable,
            Scope? scope,
            SimpleName? name = null)
        {
            IsParameter = isParameter;
            Variable = variable;
            Scope = scope;
            Name = name;
            IsMutableBinding = isMutableBinding;
            Type = type;
            if (type.ValueSemantics == ValueSemantics.Own)
                //if (type is ReferenceType referenceType && referenceType.Mutability == Mutability.Immutable)
                //    defaultSemantics = ValueSemantics.Share;
                //else
                defaultSemantics = ValueSemantics.Borrow;
            else
                defaultSemantics = type.ValueSemantics;
        }

        public VariableReference Reference(TextSpan span)
        {
            return new VariableReference(Variable, defaultSemantics, span);
        }

        public VariableReference Move(TextSpan span)
        {
            return new VariableReference(Variable, ValueSemantics.Move, span);
        }

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
            var name = Name is null ? "" : $"({Name})";
            return $"{binding} {Variable}{name}: {Type};";
        }

        public string ContextCommentString()
        {
            return Scope != null ? $" // in {Scope}" : "";
        }
    }
}
