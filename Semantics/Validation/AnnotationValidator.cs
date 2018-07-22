using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    public class AnnotationValidator
    {
        [Conditional("DEBUG")]
        public void AssertNodesAreAnnotated<T>(
            TypeSet<SyntaxBranchNode> nodeTypes,
            PackageSyntax package,
            SyntaxAnnotation<T> annotations)
        {
            foreach (var tree in package.SyntaxTrees)
                AssertNodesAreAnnotated(nodeTypes, tree.Root, annotations);
        }

        private void AssertNodesAreAnnotated<T>(
            TypeSet<SyntaxBranchNode> nodeTypes,
            SyntaxBranchNode node,
            SyntaxAnnotation<T> annotations)
        {
            if (nodeTypes.Contains(node.GetType())
                && !IsAnnotated(node, annotations))
                // Don't use Debug.Assert() becuase it doesn't play well with the test runner
                throw new Exception($"Node of type {node.GetType().Name} should have annotation of type {typeof(T).Name}, but doesn't");

            foreach (var child in node.Children)
                if (child is SyntaxBranchNode childNode)
                    AssertNodesAreAnnotated(nodeTypes, childNode, annotations);
        }

        private static bool IsAnnotated<T>(SyntaxBranchNode node, SyntaxAnnotation<T> annotations)
        {
            return annotations.TryGetValue(node, out var value) && value != null;
        }
    }
}
