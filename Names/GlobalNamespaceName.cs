using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// Represents the "name" of the global namespace i.e. nothing
    /// </summary>
    public sealed class GlobalNamespaceName : RootName
    {
        #region Singleton
        public static readonly GlobalNamespaceName Instance = new GlobalNamespaceName();

        private GlobalNamespaceName() { }
        #endregion

        public override IEnumerable<SimpleName> Segments => Enumerable.Empty<SimpleName>();

        public override MaybeQualifiedName Qualify(MaybeQualifiedName name)
        {
            return name;
        }

        public override IEnumerable<MaybeQualifiedName> NestedInNames()
        {
            return Enumerable.Empty<MaybeQualifiedName>();
        }

        public override IEnumerable<MaybeQualifiedName> NamespaceNames()
        {
            return Enumerable.Empty<MaybeQualifiedName>();
        }

        public override string ToString()
        {
            return "::";
        }
    }
}
