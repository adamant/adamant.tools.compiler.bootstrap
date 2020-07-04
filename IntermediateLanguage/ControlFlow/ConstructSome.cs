//using Adamant.Tools.Compiler.Bootstrap.Core;
//using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
//{
//    /// <summary>
//    /// A non-none value for an optional type.
//    /// </summary>
//    public class ConstructSome : Value
//    {
//        public OptionalType Type { get; }
//        public IOperand Value { get; }
//        public ConstructSome(OptionalType type, IOperand value, TextSpan span)
//            : base(span)
//        {
//            Type = type;
//            Value = value;
//        }

//        public override string ToString()
//        {
//            return $"Some({Value})";
//        }
//    }
//}
