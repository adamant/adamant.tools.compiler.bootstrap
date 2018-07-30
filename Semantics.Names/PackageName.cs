using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class PackageName : Name
    {
        public PackageName(string name)
        : base(name)
        {
            // TODO validate that the name is a valid package name
        }

        public override void GetFullName(StringBuilder builder)
        {
            builder.Append(EntityName);
            builder.Append(":"); // One colon is a bare package name, the second colon indicates the global namespace
        }

        public override void GetFullNameScope(StringBuilder builder)
        {
            GetFullName(builder);
        }
    }
}
