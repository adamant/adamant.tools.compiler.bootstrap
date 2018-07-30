using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public abstract class Name
    {
        public string EntityName { get; }
        public string FullName
        {
            get
            {
                var builder = new StringBuilder();
                GetFullName(builder);
                return builder.ToString();
            }
        }

        protected Name(string entityName)
        {
            EntityName = entityName;
        }

        public abstract void GetFullName(StringBuilder builder);

        public virtual void GetFullNameScope(StringBuilder builder)
        {
            GetFullName(builder);
            builder.Append(".");
        }

        public sealed override string ToString()
        {
            return FullName;
        }
    }
}
