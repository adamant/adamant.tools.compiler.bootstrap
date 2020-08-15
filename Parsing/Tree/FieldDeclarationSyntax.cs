using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
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
        public new Promise<FieldSymbol> Symbol { get; }
        IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
        public ITypeSyntax Type { get; }
        private IExpressionSyntax? initializer;
        [DisallowNull] public ref IExpressionSyntax? Initializer => ref initializer;
        public DataType BindingDataType => Symbol.Result.DataType;

        public FieldDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            bool mutableBinding,
            TextSpan nameSpan,
            Name name,
            ITypeSyntax type,
            IExpressionSyntax? initializer)
            : base(declaringClass, span, file, accessModifier, nameSpan, name, new Promise<FieldSymbol>())
        {
            IsMutableBinding = mutableBinding;
            Name = name;
            Type = type;
            this.initializer = initializer;
            Symbol = (Promise<FieldSymbol>)base.Symbol;
        }

        public override string ToString()
        {
            var result = $"{Name}: {Type}";
            if (Initializer != null)
                result += Initializer.ToString();
            result += ";";
            return result;
        }
    }
}
