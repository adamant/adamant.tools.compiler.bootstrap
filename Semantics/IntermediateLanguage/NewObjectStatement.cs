using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class NewObjectStatement : SimpleStatement
    {
        [NotNull] public readonly Place ResultInto;
        [NotNull] public readonly Object Type;
        [NotNull] public IReadOnlyList<Place> Arguments { get; }

        public NewObjectStatement([NotNull] Place resultInto, [NotNull] Object type, [NotNull][ItemNotNull] IEnumerable<Place> arguments)
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
