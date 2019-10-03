using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

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

        public INameExpressionSyntax? ConstructorName { get; }

        public FixedList<Argument> Arguments { get; }
        private ISymbol? constructorSymbol;

        [DisallowNull]
        public ISymbol? ConstructorSymbol
        {
            get => constructorSymbol;
            set
            {
                if (constructorSymbol != null)
                    throw new InvalidOperationException("Can't set constructor symbol repeatedly");
                constructorSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        private DataType? constructorType;
        [DisallowNull]
        public DataType? ConstructorType
        {
            get => constructorType;
            set
            {
                if (constructorType != null)
                    throw new InvalidOperationException("Can't set constructor type repeatedly");
                constructorType = value ?? throw new ArgumentNullException(nameof(value));

            }
        }

        public NewObjectExpressionSyntax(
            TextSpan span,
            ITypeNameSyntax typeSyntax,
            INameExpressionSyntax? constructorName,
            FixedList<Argument> arguments)
            : base(span)
        {
            TypeSyntax = typeSyntax;
            Arguments = arguments;
            ConstructorName = constructorName;
        }

        public override string ToString()
        {
            var name = ConstructorName != null ? "."+ConstructorName : "";
            return $"new {TypeSyntax}{name}({string.Join(", ", Arguments)})";
        }
    }
}
