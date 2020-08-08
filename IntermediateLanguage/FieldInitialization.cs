using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    /// <summary>
    /// A field initialization in an initialization caused by a constructor parameter
    /// </summary>
    public class FieldInitialization
    {
        public SimpleName ParameterName { get; }
        public Name FieldName { get; }

        public FieldInitialization(SimpleName parameterName, Name fieldName)
        {
            ParameterName = parameterName;
            FieldName = fieldName;
        }
    }
}
