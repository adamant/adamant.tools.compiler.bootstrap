using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    /// <summary>
    /// Represents the situation when the name of something is not known
    /// </summary>
    public class UnknownName : Name
    {
        #region Singleton
        [NotNull] public static readonly UnknownName Instance = new UnknownName();

        private UnknownName()
            : base("⧼Unknown Name⧽")
        {
        }
        #endregion

        public override void GetFullName([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            builder.Append(EntityName);
        }
    }
}
