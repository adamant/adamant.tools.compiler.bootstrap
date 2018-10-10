using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public abstract class Name
    {
        [NotNull] public string EntityName { get; }

        [NotNull]
        public string FullName
        {
            get
            {
                var builder = new StringBuilder();
                GetFullName(builder);
                return builder.ToString().AssertNotNull();
            }
        }

        protected Name([NotNull] string entityName)
        {
            Requires.NotNull(nameof(entityName), entityName);
            EntityName = entityName;
        }

        public abstract void GetFullName([NotNull] StringBuilder builder);

        public virtual void GetFullNameScope([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            GetFullName(builder);
            builder.Append(".");
        }

        public sealed override string ToString()
        {
            return FullName;
        }
    }
}
