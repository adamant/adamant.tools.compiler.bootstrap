using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class ParameterSyntax : Syntax, IParameterSyntax
    {
        public MaybeQualifiedName FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Name? Name { get; }
        public abstract IPromise<DataType> DataType { get; }
        public bool Unused { get; }

        protected ParameterSyntax(TextSpan span, MaybeQualifiedName fullName, Name? name)
            : base(span)
        {
            FullName = fullName;
            Name = name;
            Unused = fullName.UnqualifiedName.Text.StartsWith("_", StringComparison.Ordinal);
        }
    }
}
