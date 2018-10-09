using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    // Object types are the types created with class and struct declarations. The
    // primitive types are also object types. An object type may have generic parameters
    // that may be filled with generic arguments. An object type with generic parameters
    // but no generic arguments is *open* or *unbound*. One with generic arguments supplied
    // for all parameters is *closed* or *bound*. One with some but not all
    // arguments supplied is *partially bound*.
    public class ObjectType : DataType
    {
        [NotNull] public ObjectTypeName Name { get; }
        public bool IsMutable { get; }

        public ObjectType([NotNull] ObjectTypeName name, bool isMutable)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
            IsMutable = isMutable;
        }

        [NotNull]
        public override string ToString()
        {
            if (IsMutable)
                return "mut " + Name.FullName;

            return Name.FullName;
        }
    }
}
