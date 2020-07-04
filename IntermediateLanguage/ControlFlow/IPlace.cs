//using System;
//using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
//using ExhaustiveMatching;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
//{
//    /// <summary>
//    /// A place is a location in memory that can be assigned into
//    /// </summary>
//    [Closed(
//        typeof(FieldAccess),
//        typeof(VariableReference))]
//    public interface IPlace : IValue
//    {
//        ValueSemantics ValueSemantics { get; }
//        [Obsolete("Core variable doesn't exactly make sense for complex places")]
//        Variable CoreVariable();
//    }
//}
