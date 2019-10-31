using System;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// A place is a location in memory that can be assigned into
    /// </summary>
    [Closed(
        typeof(FieldAccess),
        typeof(VariableReference))]
    public interface IPlace : IValue
    {
        [Obsolete("Core variable doesn't exactly make sense for complex places")]
        Variable CoreVariable();
    }
}
