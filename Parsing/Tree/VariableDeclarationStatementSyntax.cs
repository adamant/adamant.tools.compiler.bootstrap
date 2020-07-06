using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class VariableDeclarationStatementSyntax : StatementSyntax, IVariableDeclarationStatementSyntax
    {
        public bool IsMutableBinding { [DebuggerStepThrough] get; }
        public Name FullName { [DebuggerStepThrough] get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public TextSpan NameSpan { [DebuggerStepThrough] get; }
        public ITypeSyntax? TypeSyntax { [DebuggerStepThrough] get; }
        public bool InferMutableType { [DebuggerStepThrough] get; }
        private DataType? type;
        DataType IBindingSymbol.Type => Type ?? throw new InvalidOperationException();

        [DisallowNull]
        public DataType? Type
        {
            [DebuggerStepThrough]
            get => type;
            set
            {
                if (type != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                type = value ?? throw new ArgumentNullException(nameof(Type),
                           "Can't set type to null");
            }
        }

        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification =
            "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax? initializer;
        [DisallowNull]
        public ref IExpressionSyntax? Initializer
        {
            [DebuggerStepThrough]
            get => ref initializer;
        }

        public bool VariableIsLiveAfter { get; set; } = true;

        public VariableDeclarationStatementSyntax(
            TextSpan span,
            bool isMutableBinding,
            Name fullName,
            TextSpan nameSpan,
            ITypeSyntax? typeSyntax,
            bool inferMutableType,
            IExpressionSyntax? initializer)
            : base(span)
        {
            IsMutableBinding = isMutableBinding;
            FullName = fullName;
            NameSpan = nameSpan;
            TypeSyntax = typeSyntax;
            InferMutableType = inferMutableType;
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
