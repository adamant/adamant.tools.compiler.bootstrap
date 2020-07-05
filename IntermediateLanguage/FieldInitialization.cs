using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    /// <summary>
    /// A field initialization in a constructor caused by a constructor parameter
    /// </summary>
    public class FieldInitialization
    {
        public SimpleName ParameterName { get; }
        public SimpleName FieldName { get; }

        public FieldInitialization(SimpleName parameterName, SimpleName fieldName)
        {
            ParameterName = parameterName;
            FieldName = fieldName;
        }
    }
}
