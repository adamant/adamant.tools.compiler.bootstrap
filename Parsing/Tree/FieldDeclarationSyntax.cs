using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FieldDeclarationSyntax : MemberDeclarationSyntax, IFieldDeclarationSyntax
    {
        public bool IsMutableBinding { get; }
        public new Name Name { get; }
        public Promise<FieldSymbol> Symbol { get; } = new Promise<FieldSymbol>();
        protected override IPromise<Symbol> SymbolPromise => Symbol;
        public ITypeSyntax TypeSyntax { get; }
        private IExpressionSyntax? initializer;
        [DisallowNull] public ref IExpressionSyntax? Initializer => ref initializer;
        public Promise<DataType> DataType { get; } = new Promise<DataType>();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType IBindingMetadata.DataType => DataType.Result;

        public FieldDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            bool mutableBinding,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            Name name,
            ITypeSyntax typeSyntax,
            IExpressionSyntax? initializer)
            : base(declaringClass, span, file, accessModifier, fullName, nameSpan, name)
        {
            IsMutableBinding = mutableBinding;
            Name = name;
            TypeSyntax = typeSyntax;
            this.initializer = initializer;
        }

        public override string ToString()
        {
            var result = $"{Name}: {TypeSyntax}";
            if (Initializer != null)
                result += Initializer.ToString();
            result += ";";
            return result;
        }
    }
}
