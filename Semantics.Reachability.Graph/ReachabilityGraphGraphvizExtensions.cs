using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    public static class ReachabilityGraphGraphvizExtensions
    {
        /// <summary>
        /// Convert the graph into DOT format supported by graphviz
        /// </summary>
        public static string ToDotFormat(this ReachabilityGraph graph)
        {
            var dot = new StringBuilder();
            var nodes = new Dictionary<MemoryPlace, string>();
            dot.AppendLine("digraph reachability {");
            dot.AppendLine("    rankdir=LR;");
            AppendStack(graph, dot, nodes);
            //AppendContextObjects(graph, dot, nodes);
            AppendObjects(graph, dot, nodes);
            AppendReferences(graph, dot, nodes);
            dot.AppendLine("}");
            return dot.ToString();
        }

        private static void AppendStack(ReachabilityGraph graph, StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            var hasCallerVariables = graph.CallerVariables.Any();
            var hasVariables = graph.Variables.Any();
            var hasTempValues = graph.TempValues.Any();
            if (!hasCallerVariables && !hasVariables && !hasTempValues) return;

            dot.AppendLine("    node [shape=plaintext];");
            dot.AppendLine("    stack [label=<");
            dot.AppendLine("        <TABLE BORDER=\"0\" CELLBORDER=\"1\" CELLSPACING=\"0\" CELLPADDING=\"4\">");
            var nextNode = 1;
            foreach (var callerVariable in graph.CallerVariables) AppendStackItem(dot, callerVariable, "cv", nodes, ref nextNode);

            if (hasCallerVariables && hasVariables) AppendStackSeparator(dot);

            nextNode = 1;
            foreach (var variable in graph.Variables) AppendStackItem(dot, variable, "v", nodes, ref nextNode);

            if ((hasCallerVariables || hasVariables) && hasTempValues) AppendStackSeparator(dot);

            nextNode = 1;
            foreach (var tempValue in graph.TempValues) AppendStackItem(dot, tempValue, "t", nodes, ref nextNode);

            dot.AppendLine("        </TABLE>>];"); // end stack
        }

        private static void AppendStackItem(
            StringBuilder dot,
            StackPlace place,
            string nodeType,
            Dictionary<MemoryPlace, string> nodes,
            ref int nextNode)
        {
            var port = nodeType + nextNode;
            nodes.Add(place, "stack:" + port + ":e");
            var label = Escape(place.ToString()!);
            dot.AppendLine($"            <TR><TD PORT=\"{port}\" ALIGN=\"LEFT\">{label}</TD></TR>");
            nextNode += 1;
        }

        private static void AppendStackSeparator(StringBuilder dot)
        {
            dot.AppendLine("            <TR><TD CELLPADDING=\"2\"></TD></TR>");
        }

        private static void AppendObjects(ReachabilityGraph graph, StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            if (!graph.Objects.Any()) return;

            dot.AppendLine("    node [shape=ellipse, peripheries=2];");

            var nextObject = 1;
            foreach (var contextObject in graph.Objects.Where(o => o.IsContext))
            {
                var name = "ctx" + nextObject;
                nodes.Add(contextObject, name);
                var label = Escape(contextObject.ToString());
                dot.AppendLine($"    {name} [label=\"{label}</U>\"];");
                nextObject += 1;
            }

            dot.AppendLine("    node [shape=ellipse, peripheries=1];");

            nextObject = 1;
            foreach (var obj in graph.Objects.Where(o => !o.IsContext))
            {
                var name = "obj" + nextObject;
                nodes.Add(obj, name);
                dot.AppendLine($"    {name} [label=\"{Escape(obj.ToString())}\"];");
                nextObject += 1;
            }
        }

        private static void AppendReferences(ReachabilityGraph graph, StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            //bool firstEdge = true;

            var sources = graph.CallerVariables
                               .Concat<MemoryPlace>(graph.Variables)
                               .Concat(graph.TempValues)
                               .Concat(graph.Objects);
            foreach (var sourceNode in sources)
            {
                foreach (var reference in sourceNode.References)
                {
                    //if (firstEdge)
                    //{
                    //    dot.AppendLine("    edge;");
                    //    firstEdge = false;
                    //}

                    AppendReference(dot, sourceNode, reference, nodes);
                }
            }
        }

        private static void AppendReference(StringBuilder dot, MemoryPlace sourceNode, Reference reference, Dictionary<MemoryPlace, string> nodes)
        {
            dot.Append($"    {nodes[sourceNode]} -> {nodes[reference.Referent]} [dir=both, ");
            switch (reference.Ownership)
            {
                default:
                    throw ExhaustiveMatch.Failed(reference.Ownership);
                case Ownership.None:
                    dot.Append("arrowtail=odiamond");
                    break;
                case Ownership.Owns:
                    dot.Append("arrowtail=diamond");
                    break;
                case Ownership.PotentiallyOwns:
                    dot.Append("arrowtail=odot");
                    break;
            }

            dot.Append(", ");
            switch (reference.DeclaredAccess)
            {
                default:
                    throw ExhaustiveMatch.Failed(reference.DeclaredAccess);
                case Access.Identify:
                    dot.Append("arrowhead=ornormal");
                    break;
                case Access.Mutable:
                    dot.Append("arrowhead=normal");
                    break;
                case Access.ReadOnly:
                    dot.Append("arrowhead=onormal");
                    break;
            }

            dot.AppendLine("];");
        }

        private static string Escape(string label)
        {
            return label.Replace("⟦", "&laquo;", StringComparison.InvariantCulture)
                        .Replace("⟧", "&raquo;", StringComparison.InvariantCulture);
        }
    }
}
