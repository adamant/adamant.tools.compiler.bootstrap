using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class VariableName : Name
    {
        public ScopeName Scope { get; }
        // TODO add declaration number

        public VariableName(ScopeName scope, string name)
            : base(name)
        {
            Scope = scope;
        }

        public override void GetFullName(StringBuilder builder)
        {
            Scope.GetFullNameScope(builder);
            builder.Append(EntityName);
        }
    }
}
