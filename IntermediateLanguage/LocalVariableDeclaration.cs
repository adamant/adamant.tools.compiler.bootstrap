using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class LocalVariableDeclaration
    {
        public readonly int Number; // The declaration number is used as its name in the IR

        // If this declaration corresponds to an argument or local variable, what it was named. Not guaranteed unique
        [CanBeNull] public SimpleName Name = null;
        public readonly bool IsParameter;
        public readonly bool MutableBinding;
        [NotNull] public readonly DataType Type;
        public bool Exists => Type != DataType.Void && Type != DataType.Never;
        [NotNull] public readonly VariableReference Reference;

        public LocalVariableDeclaration(
            bool isParameter,
            bool mutableBinding,
            [NotNull] DataType type,
            int number)
        {
            IsParameter = isParameter;
            Number = number;
            MutableBinding = mutableBinding;
            Type = type;
            Reference = new VariableReference(number);
        }

        // Useful for debugging
        public override string ToString()
        {
            var binding = MutableBinding ? "var" : "let";
            var result = $"{binding} %{Number}: {Type};";
            if (Name != null)
                result += $" // {Name}";
            return result;
        }
    }
}
