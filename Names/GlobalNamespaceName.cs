using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// Represents the "name" of the global namespace i.e. nothing
    /// </summary>
    public class GlobalNamespaceName : RootName
    {
        #region Singleton
        public static readonly GlobalNamespaceName Instance = new GlobalNamespaceName();

        private GlobalNamespaceName() { }
        #endregion

        public override IEnumerable<SimpleName> Segments => Enumerable.Empty<SimpleName>();

        public override Name Qualify(Name name)
        {
            return name;
        }

        public override IEnumerable<Name> NestedInNames()
        {
            return Enumerable.Empty<Name>();
        }

        public override IEnumerable<Name> NamespaceNames()
        {
            return Enumerable.Empty<Name>();
        }

        public override string ToString()
        {
            return "::";
        }
    }
}
