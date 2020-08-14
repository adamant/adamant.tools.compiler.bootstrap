using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class ParameterConverter : IConverter<ParameterIL>
    {
        private readonly NameMangler nameMangler;
        private readonly IConverter<DataType> typeConverter;

        public ParameterConverter(
            NameMangler nameMangler,
            IConverter<DataType> typeConverter)
        {
            this.typeConverter = typeConverter;
            this.nameMangler = nameMangler;
        }

        public string Convert(ParameterIL parameter)
        {
            var type = typeConverter.Convert(parameter.DataType);
            return parameter switch
            {
                NamedParameterIL param => $"{type} {nameMangler.Mangle(param.Symbol.Name)}",
                SelfParameterIL _ => $"{type} {nameMangler.SelfName}",
                FieldParameterIL param => $"{type} {nameMangler.Mangle(param.InitializeField.Name)}",
                _ => throw ExhaustiveMatch.Failed(parameter)
            };
        }
    }
}
