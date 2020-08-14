using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class VariableDeclarationStatementSyntax : StatementSyntax, IVariableDeclarationStatementSyntax
    {
        public bool IsMutableBinding { [DebuggerStepThrough] get; }
        public MaybeQualifiedName FullName { [DebuggerStepThrough] get; }
        public Name Name { get; }
        public Promise<int?> DeclarationNumber { get; } = new Promise<int?>();
        public Promise<VariableSymbol> Symbol { get; } = new Promise<VariableSymbol>();
        IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
        public TextSpan NameSpan { [DebuggerStepThrough] get; }
        public ITypeSyntax? Type { [DebuggerStepThrough] get; }
        public bool InferMutableType { [DebuggerStepThrough] get; }
        DataType IBindingSyntax.BindingDataType => Symbol.Result.DataType;
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
            Name name,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            ITypeSyntax? typeSyntax,
            bool inferMutableType,
            IExpressionSyntax? initializer)
            : base(span)
        {
            IsMutableBinding = isMutableBinding;
            Name = name;
            FullName = fullName;
            NameSpan = nameSpan;
            Type = typeSyntax;
            InferMutableType = inferMutableType;
            this.initializer = initializer;
        }

        public override string ToString()
        {
            var binding = IsMutableBinding ? "var" : "let";
            var type = Type != null ? ": " + Type : "";
            var initializer = Initializer != null ? " = " + Initializer : "";
            return $"{binding} {Name}{type}{initializer};";
        }
    }
}
