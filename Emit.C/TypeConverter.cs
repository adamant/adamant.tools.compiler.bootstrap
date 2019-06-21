using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class TypeConverter : IConverter<DataType>
    {
        private readonly NameMangler nameMangler;

        public TypeConverter(NameMangler nameMangler)
        {
            this.nameMangler = nameMangler;
        }

        public string Convert(DataType type)
        {
            switch (type)
            {
                case VoidType _:
                    return "void";
                case SimpleType simpleType:
                    return nameMangler.Mangle(simpleType.Name);
                case UserObjectType t:
                    return nameMangler.Mangle(t);
                case PointerType ptr:
                    var referenced = ptr.Referent.AssertKnown();
                    if (referenced == DataType.Any)
                        return "void*";
                    return Convert(referenced) + "*";
                case FunctionType functionType:
                    return $"{Convert(functionType.ReturnType)}(*)({string.Join(", ", functionType.ParameterTypes.Select(Convert))})";
                default:
                    throw NonExhaustiveMatchException.For(type);
            }
        }
    }
}
