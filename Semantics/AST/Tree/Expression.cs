using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class Expression : AbstractSyntax, IExpression
    {
        public DataType DataType { get; }

        protected Expression(TextSpan span, DataType dataType)
            : base(span)
        {
            DataType = dataType;
        }
    }
}
