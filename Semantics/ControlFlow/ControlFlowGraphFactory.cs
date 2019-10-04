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
        public ControlFlowGraph? CreateGraph(ICallableDeclarationSyntax callableDeclaration)
        {
            if (!ShouldBuildGraph(callableDeclaration)) return null;

            var fabrication = new ControlFlowFabrication(callableDeclaration.File);
            switch (callableDeclaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(callableDeclaration);
                case IMethodDeclarationSyntax method:
                    return fabrication.CreateGraph(method);
                case IConstructorDeclarationSyntax constructor:
                    return fabrication.CreateGraph(constructor);
                case IFunctionDeclarationSyntax function:
                    return fabrication.CreateGraph(function);
            }
        }

        private static bool ShouldBuildGraph(ICallableDeclarationSyntax callableDeclaration)
        {
            return callableDeclaration.Body != null // It is not abstract
                                                    /* && function.GenericParameters == null*/
                ; // It is not generic, generic functions need monomorphized
        }
    }
}
