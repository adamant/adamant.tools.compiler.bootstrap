using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class ParameterSyntax : Syntax, IParameterSyntax
    {
        public abstract bool IsMutableBinding { get; }
        public MaybeQualifiedName FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;
        public bool Unused { get; }
        public DataTypePromise DataType { get; } = new DataTypePromise();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType IBindingMetadata.DataType => DataType.Fulfilled();

        protected ParameterSyntax(TextSpan span, MaybeQualifiedName fullName)
            : base(span)
        {
            FullName = fullName;
            Unused = fullName.UnqualifiedName.Text.StartsWith("_", StringComparison.Ordinal);
        }
    }
}
