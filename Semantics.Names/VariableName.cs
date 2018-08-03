using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class VariableName : Name
    {
        public FunctionName Function { get; }
        // TODO add declaration number

        public VariableName(FunctionName function, string name)
            : base(name)
        {
            Function = function;
        }

        public override void GetFullName(StringBuilder builder)
        {
            Function.GetFullNameScope(builder);
            builder.Append(EntityName);
        }
    }
}
