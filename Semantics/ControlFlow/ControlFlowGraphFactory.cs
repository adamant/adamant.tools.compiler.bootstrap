using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    /// <summary>
    /// A factory for creating control flow graphs from callable AST nodes
    /// </summary>
    public class ControlFlowGraphFactory
    {
        //public static void BuildGraphs(FixedList<ICallableDeclarationSyntax> callableDeclarations)
        //{
        //    foreach (var callableDeclaration in callableDeclarations.Where(ShouldBuildGraph))
        //    {
        //        var builder = new ControlFlowAnalyzer();
        //        switch (callableDeclaration)
        //        {
        //            default:
        //                throw ExhaustiveMatch.Failed(callableDeclaration);
        //            case IMethodDeclarationSyntax method:
        //                builder.BuildGraph(method);
        //                break;
        //            case IConstructorDeclarationSyntax constructor:
        //                builder.BuildGraph(constructor);
        //                break;
        //            case IFunctionDeclarationSyntax function:
        //                builder.BuildGraph(function);
        //                break;
        //        }
        //    }
        //}

        public ControlFlowGraph? CreateGraph(ICallableDeclarationSyntax callableDeclaration)
        {
            if (!ShouldBuildGraph(callableDeclaration)) return null;

            var fabrication = new ControlFlowFabrication();
            switch (callableDeclaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(callableDeclaration);
                case IMethodDeclarationSyntax method:
                    fabrication.CreateGraph(method);
                    break;
                case IConstructorDeclarationSyntax constructor:
                    fabrication.CreateGraph(constructor);
                    break;
                case IFunctionDeclarationSyntax function:
                    fabrication.CreateGraph(function);
                    break;
            }

            return callableDeclaration.ControlFlow ?? throw new InvalidOperationException("control flow graph should not be null");
        }

        private static bool ShouldBuildGraph(ICallableDeclarationSyntax callableDeclaration)
        {
            return callableDeclaration.Body != null // It is not abstract
                                                    /* && function.GenericParameters == null*/
                ; // It is not generic, generic functions need monomorphized
        }
    }
}
