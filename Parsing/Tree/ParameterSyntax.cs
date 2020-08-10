using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class ParameterSyntax : Syntax, IParameterSyntax
    {
        public abstract bool IsMutableBinding { get; }
        public MaybeQualifiedName FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Name? Name { get; }
        IPromise<BindingSymbol> IParameterSyntax.Symbol => SymbolPromise;
        protected abstract IPromise<BindingSymbol> SymbolPromise { get; }
        public bool Unused { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType IBindingMetadata.DataType => SymbolPromise.Result.DataType;

        protected ParameterSyntax(TextSpan span, MaybeQualifiedName fullName, Name? name)
            : base(span)
        {
            FullName = fullName;
            Name = name;
            Unused = fullName.UnqualifiedName.Text.StartsWith("_", StringComparison.Ordinal);
        }
    }
}
