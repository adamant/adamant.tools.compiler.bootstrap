using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Factory is passed as a dependency")]
        public ControlFlowGraph? CreateGraph(IConcreteCallableDeclarationSyntax callableDeclaration)
        {
            if (!ShouldBuildGraph(callableDeclaration))
                return null;

            var fabrication = new ControlFlowFabrication(callableDeclaration.File);
            switch (callableDeclaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(callableDeclaration);
                case IConcreteMethodDeclarationSyntax method:
                    return fabrication.CreateGraph(method);
                case IConstructorDeclarationSyntax constructor:
                    return fabrication.CreateGraph(constructor);
                case IFunctionDeclarationSyntax function:
                    return fabrication.CreateGraph(function);
            }
        }

        private static bool ShouldBuildGraph(IConcreteCallableDeclarationSyntax callableDeclaration)
        {
            return callableDeclaration.Body != null // It is not abstract
                                                    /* && function.GenericParameters == null*/
                ; // It is not generic, generic functions need monomorphized
        }
    }
}
