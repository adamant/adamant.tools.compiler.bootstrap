using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FieldDeclarationSyntax : MemberDeclarationSyntax, IFieldDeclarationSyntax
    {
        public FixedList<IModiferToken> Modifiers { get; }
        public bool IsMutableBinding { get; }
        public ITypeSyntax? TypeSyntax { get; }
        private IExpressionSyntax? initializer;
        [DisallowNull] public ref IExpressionSyntax? Initializer => ref initializer;
        public TypePromise Type { get; } = new TypePromise();

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType IBindingSymbol.Type => Type.Fulfilled();

        public FieldDeclarationSyntax(
            IClassDeclarationSyntax declaringType,
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            bool mutableBinding,
            Name fullName,
            TextSpan nameSpan,
            ITypeSyntax? typeSyntax,
            IExpressionSyntax? initializer)
            : base(declaringType, span, file, fullName, nameSpan)
        {
            Modifiers = modifiers;
            IsMutableBinding = mutableBinding;
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
