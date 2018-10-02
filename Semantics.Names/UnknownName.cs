using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    /// <summary>
    /// Represents the situation when the name of something is not known
    /// </summary>
    public class UnknownName : Name
    {
        #region Singleton
        public static readonly UnknownName Instance = new UnknownName();

        private UnknownName()
            : base("⧼Unknown Name⧽")
        {
        }
        #endregion

        public override void GetFullName(StringBuilder builder)
        {
            builder.Append(EntityName);
        }
    }
}
