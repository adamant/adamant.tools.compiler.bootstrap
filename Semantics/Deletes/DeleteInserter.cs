using System;
using System.Collections;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Deletes
{
    /// <summary>
    /// Insert explicit delete statements when values are dropped
    /// </summary>
    public class DeleteInserter
    {
        public static void Transform(
            FixedList<MemberDeclarationSyntax> memberDeclarations,
            FixedDictionary<FunctionDeclarationSyntax, LiveVariables> liveness)
        {
            var inserter = new DeleteInserter();
            foreach (var function in memberDeclarations.OfType<FunctionDeclarationSyntax>())
                if (liveness.TryGetValue(function, out var functionLiveness))
                    inserter.Transform(function, functionLiveness);
        }

        private void Transform(FunctionDeclarationSyntax function, LiveVariables liveness)
        {
            var graph = function.ControlFlow;
            var builder = new ControlFlowGraphBuilder(graph.VariableDeclarations);

            foreach (var block in graph.BasicBlocks)
            {
                var blockBuilder = builder.NewBlock();
                if (block.Number != blockBuilder.BlockNumber)
                    throw new Exception("New block number does not match old block number");

                foreach (var statement in block.ExpressionStatements)
                {
                    // Copy the existing statement
                    blockBuilder.Add(statement);

                    var before = liveness.Before(statement);
                    var after = liveness.After(statement);
                    // dead = live before and not live after
                    var dead = ((BitArray)after.Clone()).Not().And(before);
                    foreach (var variableNumber in dead.TrueIndexes())
                    {
                        var variable = graph.VariableDeclarations[variableNumber];
                        if (variable.Type is ObjectType type
                            && type.IsOwned)
                        {
                            // The delete happens after the last statement
                            var span = new TextSpan(statement.Span.End, 0);
                            blockBuilder.AddDelete(new VariableReference(variable.Number, VariableReferenceKind.Borrow, span), type, span);
                        }
                    }
                }
                // TODO what about if there is a variable delete after the terminator?
                blockBuilder.Add(block.Terminator);
            }

            function.ControlFlow = builder.Build();
        }
    }
}
