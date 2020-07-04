//using Adamant.Tools.Compiler.Bootstrap.Core;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
//{
//    public class ActionStatement : ExpressionStatement
//    {
//        public Value Value { get; }

//        public ActionStatement(Value value, TextSpan span, Scope scope)
//            : base(span, scope)
//        {
//            Value = value;
//        }

//        public override Statement Clone()
//        {
//            return new ActionStatement(Value, Span, Scope);
//        }

//        // Useful for debugging
//        public override string ToStatementString()
//        {
//            return $"{Value};";
//        }
//    }
//}
