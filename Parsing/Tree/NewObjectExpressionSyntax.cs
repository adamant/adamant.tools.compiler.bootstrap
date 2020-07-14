using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NewObjectExpressionSyntax : ExpressionSyntax, INewObjectExpressionSyntax
    {
        /// <summary>
        /// Note that this could represent a named or unnamed constructor. So
        /// for an unnamed constructor, it is really the type name. Conceptually
        /// though, the type name is the name of the unnamed constructor. Thus,
        /// this expression's type could be either an object type, or member type.
        /// </summary>
        public ITypeNameSyntax TypeSyntax { get; }

        public ICallableNameSyntax? ConstructorName { get; }

        public FixedList<IArgumentSyntax> Arguments { get; }
        private IFunctionSymbol? constructorSymbol;

        [DisallowNull]
        public IFunctionSymbol? ConstructorSymbol
        {
            get => constructorSymbol;
            set
            {
                if (constructorSymbol != null)
                    throw new InvalidOperationException("Can't set constructor symbol repeatedly");
                constructorSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public NewObjectExpressionSyntax(
            TextSpan span,
            ITypeNameSyntax typeSyntax,
            ICallableNameSyntax? constructorName,
            FixedList<IArgumentSyntax> arguments)
            : base(span, ExpressionSemantics.Acquire)
        {
            TypeSyntax = typeSyntax;
            Arguments = arguments;
            ConstructorName = constructorName;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            var name = ConstructorName != null ? "."+ConstructorName : "";
            return $"new {TypeSyntax}{name}({string.Join(", ", Arguments)})";
        }
    }
}
