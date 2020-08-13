using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
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
        public ITypeNameSyntax Type { get; }

        public IInvocableNameSyntax? ConstructorName { get; }

        public FixedList<IArgumentSyntax> Arguments { get; }
        private IFunctionMetadata? constructorSymbol;

        [DisallowNull]
        public IFunctionMetadata? ReferencedConstructor
        {
            get => constructorSymbol;
            set
            {
                if (constructorSymbol != null)
                    throw new InvalidOperationException($"Can't set {nameof(ReferencedConstructor)} repeatedly");
                constructorSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public NewObjectExpressionSyntax(
            TextSpan span,
            ITypeNameSyntax typeSyntax,
            IInvocableNameSyntax? constructorName,
            FixedList<IArgumentSyntax> arguments)
            : base(span, ExpressionSemantics.Acquire)
        {
            Type = typeSyntax;
            Arguments = arguments;
            ConstructorName = constructorName;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            var name = ConstructorName != null ? "."+ConstructorName : "";
            return $"new {Type}{name}({string.Join(", ", Arguments)})";
        }
    }
}
