using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class LocalVariableDeclaration
    {
        public readonly Variable Variable; // The declaration number is used as its name in the IR

        // If this declaration corresponds to an argument or local variable, what it was named. Not guaranteed unique
        public readonly SimpleName Name;
        public readonly bool IsParameter;
        public readonly bool MutableBinding;
        public readonly DataType Type;
        public bool Exists => Type != DataType.Void && Type != DataType.Never;
        public readonly VariableReference AssignReference;
        public readonly VariableReference Reference;

        public LocalVariableDeclaration(
            bool isParameter,
            bool mutableBinding,
            DataType type,
            Variable variable,
            SimpleName name = null)
        {
            IsParameter = isParameter;
            Variable = variable;
            Name = name;
            MutableBinding = mutableBinding;
            Type = type;
            // TODO correct span?
            AssignReference = new VariableReference(variable, VariableReferenceKind.Assign, new TextSpan(0, 0));
            // TODO should this be `Share` if the type is immutable?
            VariableReferenceKind kind;
            if (type is ObjectType objectType && objectType.Mutability == Mutability.Immutable)
                kind = VariableReferenceKind.Share;
            else
                kind = VariableReferenceKind.Borrow;
            Reference = new VariableReference(variable, kind, new TextSpan(0, 0));
        }

        // Useful for debugging
        public override string ToString()
        {
            var binding = MutableBinding ? "var" : "let";
            var result = $"{binding} {Variable}: {Type};";
            if (Name != null)
                result += $" // {Name}";
            return result;
        }
    }
}
