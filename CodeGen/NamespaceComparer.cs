using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen
{
    public class NamespaceComparer : IComparer<string>
    {
        #region Singleton
        public static NamespaceComparer Instance { get; } = new NamespaceComparer();

        private NamespaceComparer() { }
        #endregion

        public int Compare(string? x, string? y)
        {
            var xParts = x!.Split('.');
            var yParts = y!.Split('.');

            if (xParts[0] == "System" && yParts[0] != "System") return -1;
            if (xParts[0] != "System" && yParts[0] == "System") return 1;

            var length = Math.Min(xParts.Length, yParts.Length);
            for (int i = 0; i < length; i++)
            {
                var cmp = string.Compare(xParts[i], yParts[i], StringComparison.InvariantCulture);
                if (cmp != 0) return cmp;
            }

            return xParts.Length.CompareTo(yParts.Length);
        }
    }
}
