using System;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class NotNullExtension
    {
        /// <summary>
        /// Sometimes the system can't figure out that something isn't null.
        /// This let's us tell it that it is.
        /// </summary>
        [NotNull]
        public static T AssertNotNull<T>([CanBeNull] this T value)
            where T : class
        {
            return value ?? throw new ArgumentNullException();
        }
    }
}
