using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class ParameterConverter : IConverter<Parameter>
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

        public string Convert(Parameter parameter)
        {
            var type = typeConverter.Convert(parameter.DataType);
            return parameter switch
            {
                NamedParameter param => $"{type} {nameMangler.Mangle(param.Symbol.Name)}",
                SelfParameter _ => $"{type} {nameMangler.SelfName}",
                FieldParameter param => $"{type} {nameMangler.Mangle(param.InitializeField.Name)}",
                _ => throw ExhaustiveMatch.Failed(parameter)
            };
        }
    }
}
