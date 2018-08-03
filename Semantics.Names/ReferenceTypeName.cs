using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class ReferenceTypeName : ObjectTypeName
    {
        public ScopeName Scope { get; }

        public ReferenceTypeName(ScopeName scope, string name)
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
