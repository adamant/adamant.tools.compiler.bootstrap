//using Adamant.Tools.Compiler.Bootstrap.Core;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
//{
//    public class AssignmentStatement : ExpressionStatement
//    {
//        public IPlace Place { get; }
//        public IValue Value { get; }

//        public AssignmentStatement(IPlace place, IValue value, TextSpan span, Scope scope)
//            : base(span, scope)
//        {
//            Place = place;
//            Value = value;
//        }

//        public override Statement Clone()
//        {
//            return new AssignmentStatement(Place, Value, Span, Scope);
//        }

//        // Useful for debugging
//        public override string ToStatementString()
//        {
//            return $"{Place} = {Value};";
//        }
//    }
//}
