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
    // Plan for inserting deletes:
    // ---------------------------
    // When generating the CFG, output statements for when variables go out of scope. Then track another level
    // of liveness between alive and dead. This would be a new state, perhaps called "liminal" or "pending", which
    // indicated that the variable would not be used again, but may need to exist as the owner of something borrowed
    // until all outstanding borrows are resolved. Delete statements could then be inserted after borrow checking
    // at the point where values are no longer used. The variable leaving scope statements could then be ignored
    // or removed.
    // TODO inserting deletes based only on liveness led to issues.
    // There are cases when a variable is no longer directly used, but another variable has borrowed the value.
    // The owner then shows as dead, so we inserted the delete. But really, we needed to wait until the
    // borrow is gone to delete it. However, we can't just wait for all borrows to be dead to insert a delete
    // because things are guaranteed to be deleted when the owner goes out of scope. A borrow that extends
    // beyond the scope should be an error. We had been detecting this by noticing the borrow claim extended
    // past the delete statement. That worked, because the owning variable must always be dead after the scope
    // because there are no references to it outside the scope.
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
                if (block.Name != blockBuilder.BlockName)
                    throw new Exception("New block number does not match old block number");

                foreach (var statement in block.ExpressionStatements)
                {
                    // Copy the existing statement
                    blockBuilder.Add(statement);

                    var liveBefore = liveness.Before(statement);
                    var liveAfter = liveness.After(statement);
                    // died = live before and not live after
                    var died = ((BitArray)liveAfter.Clone()).Not().And(liveBefore);

                    foreach (var variableNumber in died.TrueIndexes())
                    {
                        var variable = graph.VariableDeclarations[variableNumber];
                        if (variable.Type is UserObjectType type && type.IsOwned)
                        {
                            // The delete happens after the statement
                            var span = new TextSpan(statement.Span.End, 0);
                            blockBuilder.AddDelete(new VariableReference(variable.Variable, ValueSemantics.Own, span), type, span, statement.Scope);
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
