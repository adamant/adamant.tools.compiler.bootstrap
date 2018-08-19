using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IL
{
    public class LocalVariableDeclaration
    {
        public readonly int Number; // The declaration number is used as its name int the IR

        // If this declaration corresponds to an argument or local variable, what it was named. Not guaranteed unique
        public readonly string Name = null;
        public readonly bool MutableBinding;
        public readonly DataType Type;

        public LocalVariableDeclaration(bool mutableBinding, DataType type, int number)
        {
            MutableBinding = mutableBinding;
            Type = type;
            Number = number;
        }
    }
}
