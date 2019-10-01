using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class VariableDeclarationStatementSyntax : StatementSyntax, IVariableDeclarationStatementSyntax
    {
        public bool IsMutableBinding { get; }
        public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public TextSpan NameSpan { get; }
        public ITypeSyntax TypeSyntax { get; }
        private DataType? type;


        DataType IBindingSymbol.Type => Type ?? throw new InvalidOperationException();
        public DataType? Type
        {
            get => type;
            set
            {
                if (type != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                type = value ?? throw new ArgumentNullException(nameof(Type),
                           "Can't set type to null");
            }
        }

        private IExpressionSyntax initializer;
        public IExpressionSyntax Initializer => initializer;
        public ref IExpressionSyntax InitializerRef => ref initializer;

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        SymbolSet ISymbol.ChildSymbols => SymbolSet.Empty;

        public VariableDeclarationStatementSyntax(
            TextSpan span,
            bool isMutableBinding,
            Name fullName,
            TextSpan nameSpan,
            ITypeSyntax typeSyntax,
            IExpressionSyntax initializer)
            : base(span)
        {
            IsMutableBinding = isMutableBinding;
            FullName = fullName;
            NameSpan = nameSpan;
            TypeSyntax = typeSyntax;
            this.initializer = initializer;
        }

        public override string ToString()
        {
            var binding = IsMutableBinding ? "var" : "let";
            var type = TypeSyntax != null ? ": " + TypeSyntax : "";
            var initializer = Initializer != null ? " = " + Initializer : "";
            return $"{binding} {Name}{type}{initializer};";
        }
    }
}
