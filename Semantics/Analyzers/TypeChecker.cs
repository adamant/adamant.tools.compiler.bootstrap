//using System.Collections.Generic;
//using System.Linq;
//using Adamant.Tools.Compiler.Bootstrap.AST;
//using Adamant.Tools.Compiler.Bootstrap.Framework;
//using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses;
//using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
//using Adamant.Tools.Compiler.Bootstrap.Tokens;
//using Adamant.Tools.Compiler.Bootstrap.Types;
//using JetBrains.Annotations;

//namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
//{
//    public class TypeChecker
//    {
//        public static void CheckDeclarations(
//            [NotNull] IList<MemberDeclarationAnalysis> analyses)
//        {
//            foreach (var analysis in analyses)
//                switch (analysis)
//                {
//                    case FunctionDeclarationAnalysis f:
//                        CheckFunctionSignature(f);
//                        break;
//                    case TypeDeclarationAnalysis t:
//                        CheckTypeDeclaration(t);
//                        break;
//                    default:
//                        throw NonExhaustiveMatchException.For(analysis);
//                }

//            // Now that the signatures are done, we can check the bodies
//            foreach (var function in analyses.OfType<FunctionDeclarationAnalysis>())
//            {
//                CheckFunctionBody(function);
//            }
//        }

//        private static void CheckFunctionSignature([NotNull] FunctionDeclarationAnalysis function)
//        {
//            // Check the signature first
//            function.Type.BeginComputing();
//            function.ReturnType.BeginComputing();

//            var expressionChecker = new ExpressionTypeChecker(function.Context.File, DataType.Unknown, function.Diagnostics);

//            if (function.IsGeneric)
//                CheckGenericParameters(function.GenericParameters.NotNull(), expressionChecker);
//            CheckParameters(function, expressionChecker);

//            var returnType = function.ReturnTypeExpression != null
//                ? expressionChecker.EvaluateTypeExpression(function.ReturnTypeExpression)
//                : ObjectType.Void;
//            function.ReturnType.Computed(returnType);

//            var functionType = returnType;
//            functionType = new FunctionType(function.Parameters.Select(p => p.Type.AssertComputed()), functionType);

//            if (function.IsGeneric && function.GenericParameters.NotNull().Any())
//                functionType = new MetaFunctionType(function.GenericParameters.NotNull().Select(p => p.Type.AssertComputed()), functionType);

//            function.Type.Computed(functionType);
//        }

//        private static void CheckFunctionBody([NotNull] FunctionDeclarationAnalysis function)
//        {
//            var expressionChecker = new ExpressionTypeChecker(function.Context.File, function.ReturnType.AssertComputed(), function.Diagnostics);

//            foreach (var statement in function.Statements)
//                expressionChecker.CheckStatement(statement);
//        }

//        private static void CheckGenericParameters(
//            [NotNull, ItemNotNull] FixedList<GenericParameterAnalysis> genericParameters,
//            [NotNull] ExpressionTypeChecker expressionChecker)
//        {
//            foreach (var parameter in genericParameters)
//            {
//                parameter.Type.BeginComputing();
//                parameter.Type.Computed(parameter.TypeExpression == null ?
//                    ObjectType.Type
//                    : expressionChecker.EvaluateTypeExpression(parameter.TypeExpression));
//            }
//        }

//        private static void CheckParameters(
//            [NotNull] FunctionDeclarationAnalysis function,
//            [NotNull] ExpressionTypeChecker expressionChecker)
//        {
//            foreach (var parameter in function.Parameters)
//            {
//                parameter.Type.BeginComputing();
//                if (parameter.TypeExpression != null)
//                    parameter.Type.Computed(expressionChecker.EvaluateTypeExpression(parameter.TypeExpression));
//                else
//                {
//                    function.Diagnostics.Add(TypeError.NotImplemented(parameter.Context.File,
//                        parameter.Syntax.Span, "Self parameters not implemented"));
//                    parameter.Type.Computed(DataType.Unknown);
//                }
//            }
//        }

//        /// <summary>
//        /// If the type has not been checked, this checks it and returns it.
//        /// Also watches for type cycles
//        /// </summary>
//        public static void CheckTypeDeclaration(
//            [NotNull] TypeDeclarationAnalysis declaration)
//        {
//            switch (declaration.Type.State)
//            {
//                case AnalysisState.BeingComputed:
//                    declaration.Diagnostics.Add(TypeError.CircularDefinition(declaration.Context.File, declaration.Syntax.NameSpan, declaration.Name));
//                    return;
//                case AnalysisState.Computed:
//                    return;   // We have already checked it
//                case AnalysisState.NotComputed:
//                    // we need to compute it
//                    break;
//            }

//            declaration.Type.BeginComputing();

//            var expressionChecker = new ExpressionTypeChecker(declaration.Context.File, DataType.Unknown, declaration.Diagnostics);

//            FixedList<DataType> genericParameterTypes = null;
//            if (declaration.IsGeneric)
//            {
//                var genericParameters = declaration.GenericParameters.NotNull();
//                CheckGenericParameters(genericParameters, expressionChecker);
//                genericParameterTypes = genericParameters.Select(p => p.Type.AssertComputed()).ToFixedList();
//            }
//            switch (declaration.Syntax)
//            {
//                case ClassDeclarationSyntax classDeclaration:
//                    var classType = new ObjectType(declaration.Name, true,
//                        classDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
//                        genericParameterTypes);
//                    declaration.Type.Computed(new Metatype(classType));
//                    break;
//                case StructDeclarationSyntax structDeclaration:
//                    var structType = new ObjectType(declaration.Name, false,
//                        structDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
//                        genericParameterTypes);
//                    declaration.Type.Computed(new Metatype(structType));
//                    break;
//                case EnumStructDeclarationSyntax enumStructDeclaration:
//                    var enumStructType = new ObjectType(declaration.Name, false,
//                        enumStructDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
//                        genericParameterTypes);
//                    declaration.Type.Computed(new Metatype(enumStructType));
//                    break;
//                case EnumClassDeclarationSyntax enumStructDeclaration:
//                    var enumClassType = new ObjectType(declaration.Name, true,
//                        enumStructDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
//                        genericParameterTypes);
//                    declaration.Type.Computed(new Metatype(enumClassType));
//                    break;
//                case TraitDeclarationSyntax declarationSyntax:
//                    var type = new ObjectType(declaration.Name, true,
//                        declarationSyntax.Modifiers.Any(m => m is IMutableKeywordToken),
//                        genericParameterTypes);
//                    declaration.Type.Computed(new Metatype(type));
//                    break;
//                default:
//                    throw NonExhaustiveMatchException.For(declaration.Syntax);
//            }
//        }
//    }
//}
