using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;

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
        public ObjectTypeName Name { get; }
        public bool IsMutable;

        public ObjectType(ObjectTypeName name, bool isMutable)
        {
            Name = name;
            IsMutable = isMutable;
        }

        public override string ToString()
        {
            if (IsMutable)
                return "mut " + Name.FullName;

            return Name.FullName;
        }
    }
}
