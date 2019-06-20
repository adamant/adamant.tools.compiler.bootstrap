using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class LocalVariableDeclaration
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
        private readonly VariableReferenceKind defaultReferenceKind;

        public LocalVariableDeclaration(
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
            if (type is UserObjectType objectType && objectType.Mutability == Mutability.Immutable)
                defaultReferenceKind = VariableReferenceKind.Share;
            else
                defaultReferenceKind = VariableReferenceKind.Borrow;
        }

        public VariableReference Reference(TextSpan span)
        {
            return new VariableReference(Variable, defaultReferenceKind, span);
        }

        public VariableReference AssignReference(TextSpan span)
        {
            return new VariableReference(Variable, VariableReferenceKind.Assign, span);
        }

        // Useful for debugging
        public override string ToString()
        {
            var binding = IsMutableBinding ? "var" : "let";
            var result = $"{binding} {Variable}: {Type};";
            if (Scope != null || Name != null)
                result += " //";
            if (Scope != null)
                result += $" in {Scope}";
            if (Name != null)
                result += $" for {Name}";
            return result;
        }
    }
}
