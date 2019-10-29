using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class ParameterSyntax : Syntax, IParameterSyntax
    {
        public bool IsMutableBinding { get; }
        public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;
        public bool Unused { get; }
        public TypePromise Type { get; } = new TypePromise();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType IBindingSymbol.Type => Type.Fulfilled();

        protected ParameterSyntax(TextSpan span, bool isMutableBinding, Name fullName)
            : base(span)
        {
            IsMutableBinding = isMutableBinding;
            FullName = fullName;
            Unused = fullName.UnqualifiedName.Text.StartsWith("_", StringComparison.Ordinal);
        }
    }
}
