using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class ParameterConverter : IConverter<Parameter>
    {
        [NotNull] private readonly NameMangler nameMangler;
        [NotNull] private readonly IConverter<DataType> typeConverter;

        public ParameterConverter(
            [NotNull] NameMangler nameMangler,
            [NotNull] IConverter<DataType> typeConverter)
        {
            this.typeConverter = typeConverter;
            this.nameMangler = nameMangler;
        }

        public string Convert([NotNull] Parameter parameter)
        {
            var type = typeConverter.Convert(parameter.Type.AssertNotNull());
            var name = nameMangler.Mangle(parameter.Name);
            return parameter.MutableBinding ? $"{type} ᵢ{name}" : $"const {type} ᵢ{name}";
        }
    }
}
