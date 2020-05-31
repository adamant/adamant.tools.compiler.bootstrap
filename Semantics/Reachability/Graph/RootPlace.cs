using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A root place is a place that
    /// </summary>
    internal abstract class RootPlace : Place
    {
        public void MarkReadOnlyObjects()
        {
            var readOnlyReferences = References
                                          .Where(r => r.IsUsed && r.DeclaredAccess == Access.ReadOnly);
            foreach (var reference in readOnlyReferences)
                reference.Referent.MarkReadOnly();
        }
    }
}
