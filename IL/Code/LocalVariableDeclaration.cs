using Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IL
{
    public class LocalVariableDeclaration
    {
        public readonly int Number; // The declaration number is used as its name int the IR

        // If this declaration corresponds to an argument or local variable, what it was named. Not guaranteed unique
        public string Name = null;
        public readonly bool MutableBinding;
        public readonly DataType Type;
        public readonly VariableReference Reference;

        public LocalVariableDeclaration(bool mutableBinding, DataType type, int number)
        {
            Number = number;
            MutableBinding = mutableBinding;
            Type = type;
            Reference = new VariableReference(number);
        }
    }
}
