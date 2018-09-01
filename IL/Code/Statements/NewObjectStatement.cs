using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements
{
    public class NewObjectStatement : SimpleStatement
    {
        public readonly LValue ResultInto;
        public readonly DataType Type;
        public IReadOnlyList<LValue> Arguments { get; }

        public NewObjectStatement(LValue resultInto, DataType type, IEnumerable<LValue> arguments)
        {
            Arguments = arguments.ToList().AsReadOnly();
            ResultInto = resultInto;
            Type = type;
        }

        internal override void ToString(AsmBuilder builder)
        {
            builder.AppendLine($"{ResultInto} = new {Type}({string.Join(',', Arguments)})");
        }
    }
}
