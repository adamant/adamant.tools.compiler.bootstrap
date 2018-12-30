using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        /// <summary>
        /// Note that this could represent a named or unnamed constructor. So
        /// for an unnamed constructor, it is really the type name. Conceptually
        /// though, the type name is the name of the unnamed constructor. Thus,
        /// this expression's type could be either an object type, or member type.
        /// </summary>
        public ExpressionSyntax Constructor { get; }
        public FixedList<ArgumentSyntax> Arguments { get; }
        private ISymbol constructorSymbol;
        public ISymbol ConstructorSymbol
        {
            get => constructorSymbol;
            set
            {
                if (constructorSymbol != null) throw new InvalidOperationException("Can't set constructor symbol repeatedly");
                constructorSymbol = value ?? throw new ArgumentException();
            }
        }
        private DataType constructorType;
        public DataType ConstructorType
        {
            get => constructorType;
            set
            {
                if (constructorType != null) throw new InvalidOperationException("Can't set constructor type repeatedly");
                constructorType = value ?? throw new ArgumentException();
            }
        }

        public NewObjectExpressionSyntax(
            TextSpan span,
            ExpressionSyntax constructor,
            FixedList<ArgumentSyntax> arguments)
            : base(span)
        {
            Constructor = constructor;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"new {Constructor}({string.Join(", ", Arguments)})";
        }
    }
}
