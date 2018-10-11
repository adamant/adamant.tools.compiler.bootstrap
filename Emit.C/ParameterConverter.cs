using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class ParameterConverter : IConverter<Parameter>
    {
        public string Convert([NotNull] Parameter parameter)
        {
            throw new NotImplementedException();
            //if (parameter.MutableBinding)
            //    return $"{Convert(parameter.Type.Type)} {parameter.Name}";
            //else
            //    return $"const {Convert(parameter.Type.Type)} {parameter.Name}";
        }
    }
}
