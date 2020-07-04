//using Adamant.Tools.Compiler.Bootstrap.Core;
//using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
//using Adamant.Tools.Compiler.Bootstrap.Names;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
//{
//    public class FieldAccess : Value, IPlace
//    {
//        public IPlace Expression { get; }
//        public Name Field { get; }
//        public ValueSemantics ValueSemantics { get; }

//        public FieldAccess(IPlace expression, Name field, ValueSemantics valueSemantics, TextSpan span)
//            : base(span)
//        {
//            Expression = expression;
//            Field = field;
//            ValueSemantics = valueSemantics;
//        }

//        public Variable CoreVariable()
//        {
//#pragma warning disable 618
//            return Expression.CoreVariable();
//#pragma warning restore 618
//        }

//        public override string ToString()
//        {
//            return $"{ValueSemantics.Action()} ({Expression}).{Field.UnqualifiedName}";
//        }
//    }
//}
