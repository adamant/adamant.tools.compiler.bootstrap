//using System.Collections.Generic;
//using System.Linq;
//using Adamant.Tools.Compiler.Bootstrap.Core;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
//{
//    public class ReturnStatement : BlockTerminatorStatement
//    {
//        public ReturnStatement(TextSpan span, Scope scope)
//            : base(span, scope)
//        {
//        }

//        public override IEnumerable<BasicBlockName> OutBlocks()
//        {
//            return Enumerable.Empty<BasicBlockName>();
//        }

//        public override Statement Clone()
//        {
//            return new ReturnStatement(Span, Scope);
//        }

//        public override string ToStatementString()
//        {
//            return "return;";
//        }
//    }
//}
