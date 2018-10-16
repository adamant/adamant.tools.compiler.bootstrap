using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class TypeChecker
    {
        [NotNull] private readonly NameBinder nameBinder;

        public TypeChecker([NotNull] NameBinder nameBinder)
        {
            this.nameBinder = nameBinder;
        }

        public void CheckTypes([NotNull] IList<DeclarationAnalysis> analyses)
        {
            foreach (var analysis in analyses)
            {
                switch (analysis)
                {
                    case FunctionAnalysis f:
                        CheckTypes(f);
                        break;
                    case TypeAnalysis t:
                        CheckTypes(t);
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(analysis);
                }
            }
        }

        private void CheckTypes([NotNull] FunctionAnalysis function)
        {
            var parameterTypes = function.Syntax.Parameters.Nodes().Select(n => n.TypeExpression);
            foreach (var (parameter, type) in function.Semantics.Parameters.Zip(parameterTypes))
                parameter.Type = ResolveType(type);

            function.Semantics.ReturnType = ResolveType(function.Syntax.ReturnTypeExpression);
        }

        private static void CheckTypes(TypeAnalysis type)
        {
            // TODO
        }

        [NotNull]
        private static DataType ResolveType([NotNull] ExpressionSyntax typeExpression)
        {
            switch (typeExpression)
            {
                case PrimitiveTypeSyntax primitive:
                    switch (primitive.Keyword)
                    {
                        case IntKeywordToken _:
                            return ObjectType.Int;
                        case UIntKeywordToken _:
                            return ObjectType.UInt;
                        case ByteKeywordToken _:
                            return ObjectType.Byte;
                        case SizeKeywordToken _:
                            return ObjectType.Size;
                        case VoidKeywordToken _:
                            return ObjectType.Void;
                        case BoolKeywordToken _:
                            return ObjectType.Bool;
                        case StringKeywordToken _:
                            return ObjectType.String;
                        case NeverKeywordToken _:
                            return ObjectType.Never;
                        default:
                            throw NonExhaustiveMatchException.For(primitive.Keyword);
                    }
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }
    }
}
