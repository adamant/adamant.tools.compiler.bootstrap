using System;

namespace Xunit
{
    public partial class Assert
    {
        public static T OfType<T>(object @object)
        {
            return IsAssignableFrom<T>(@object);
        }

        public static void OfType(Type expectedType, object @object)
        {
            IsAssignableFrom(expectedType, @object);
        }
    }
}
