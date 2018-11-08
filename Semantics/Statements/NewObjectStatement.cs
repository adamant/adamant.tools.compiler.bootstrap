using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public class NewObjectStatement : SimpleStatement
    {
        [NotNull] public readonly LValue ResultInto;
        [NotNull] public readonly Object Type;
        [NotNull] public IReadOnlyList<LValue> Arguments { get; }

        public NewObjectStatement([NotNull] LValue resultInto, [NotNull] Object type, [NotNull][ItemNotNull] IEnumerable<LValue> arguments)
        {
            Requires.NotNull(nameof(resultInto), resultInto);
            Requires.NotNull(nameof(type), type);
            Requires.NotNull(nameof(arguments), arguments);
            ResultInto = resultInto;
            Type = type;
            Arguments = arguments.ToReadOnlyList();
        }
    }
}
