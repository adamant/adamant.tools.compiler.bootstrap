using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A graph of the possible references between places in a function. Also
    /// answers questions about the current mutability of objects etc.
    /// </summary>
    public class ReachabilityGraph
    {
        private readonly Dictionary<IBindingSymbol, CallerVariable> callerVariables = new Dictionary<IBindingSymbol, CallerVariable>();
        private readonly Dictionary<IBindingSymbol, Variable> variables = new Dictionary<IBindingSymbol, Variable>();

        private readonly Dictionary<ISyntax, ContextObject> contextObjects = new Dictionary<ISyntax, ContextObject>();
        private readonly Dictionary<ISyntax, Object> objects = new Dictionary<ISyntax, Object>();

        private readonly HashSet<TempValue> tempValues = new HashSet<TempValue>();

        internal IReadOnlyCollection<CallerVariable> CallerVariables => callerVariables.Values;
        internal IReadOnlyCollection<Variable> Variables => variables.Values;
        internal IReadOnlyCollection<ContextObject> ContextObjects => contextObjects.Values;
        internal IReadOnlyCollection<Object> Objects => objects.Values;

        #region Add/Remove Methods
        /// <returns>The added local variable for the parameter</returns>
        public Variable? AddParameter(IParameterSyntax parameter)
        {
            // Non-reference types don't participate in reachability (yet)
            var referenceType = parameter.Type.Known().UnderlyingReferenceType();
            if (referenceType is null)
                return null;

            CallerVariable? callerVariable = null;
            var localVariable = new Variable(parameter);

            var capability = referenceType.ReferenceCapability;
            switch (capability)
            {
                default:
                    throw ExhaustiveMatch.Failed(capability);
                case ReferenceCapability.IsolatedMutable:
                case ReferenceCapability.Isolated:
                {
                    // Isolated parameters are fully independent of the caller
                    var reference = Reference.ToNewParameterObject(parameter);
                    localVariable.AddReference(reference);
                }
                break;
                case ReferenceCapability.Owned:
                case ReferenceCapability.OwnedMutable:
                case ReferenceCapability.Held:
                case ReferenceCapability.HeldMutable:
                {
                    var reference = Reference.ToNewParameterObject(parameter);
                    localVariable.AddReference(reference);
                    var referencedObject = reference.Referent;

                    // Object to represent the bounding of the lifetime
                    callerVariable = CallerVariable.CreateForParameterWithObject(parameter);
                    referencedObject.ShareFrom(callerVariable);
                }
                break;
                case ReferenceCapability.Borrowed:
                {
                    callerVariable = CallerVariable.CreateForParameterWithObject(parameter);
                    localVariable.BorrowFrom(callerVariable);
                }
                break;
                case ReferenceCapability.Shared:
                {
                    callerVariable = CallerVariable.CreateForParameterWithObject(parameter);
                    localVariable.ShareFrom(callerVariable);
                }
                break;
                case ReferenceCapability.Identity:
                {
                    callerVariable = CallerVariable.CreateForParameterWithObject(parameter);
                    localVariable.IdentityFrom(callerVariable);
                }
                break;
            }


            if (!(callerVariable is null))
            {
                callerVariables.Add(callerVariable.Symbol, callerVariable);
                AddReferences(callerVariable);
            }

            Add(localVariable);

            return localVariable;
        }

        public Variable? AddField(IFieldDeclarationSyntax fieldDeclaration)
        {
            var referenceType = fieldDeclaration.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var variable = Variable.ForField(fieldDeclaration);
            Add(variable);
            return variable;
        }

        public Variable? AddVariable(IBindingSymbol bindingSymbol)
        {
            var referenceType = bindingSymbol.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var variable = Variable.Declared(bindingSymbol);
            Add(variable);
            return variable;
        }

        public void Add(HeapPlace place)
        {
            switch (place)
            {
                default:
                    throw ExhaustiveMatch.Failed(place);
                case ContextObject contextObject:
                    if (contextObjects.TryAdd(contextObject.OriginSyntax, contextObject))
                        AddReferences(contextObject);
                    break;
                case Object @object:
                    if (objects.TryAdd(@object.OriginSyntax, @object))
                        AddReferences(@object);
                    break;
            }
        }

        public void Add(TempValue? temp)
        {
            if (temp is null) return; // for convenience
            if (tempValues.Add(temp))
                AddReferences(temp);
        }

        private void Add(Variable? variable)
        {
            if (variable is null) return; // for convenience
            variables.Add(variable.Symbol, variable);
            AddReferences(variable);
        }

        private void AddReferences(MemoryPlace place)
        {
            foreach (var reference in place.References)
                Add(reference.Referent);
        }

        public bool EndVariableScope(IBindingSymbol variable)
        {
            if (!variables.Remove(variable, out var place)) return false;
            place.Free();
            return true;
        }

        public bool Remove(TempValue tempValue)
        {
            // Make sure this is released
            tempValue.Free();
            return tempValues.Remove(tempValue);
        }

        public void Remove(IEnumerable<TempValue?> values)
        {
            foreach (var tempValue in values)
                if (!(tempValue is null))
                    Remove(tempValue);
        }
        #endregion

        public Variable GetVariableFor(IBindingSymbol variableSymbol)
        {
            // Variable needs to have already been declared
            return variables[variableSymbol];
        }
        public Variable? TryGetVariableFor(IBindingSymbol variableSymbol)
        {
            return variables.TryGetValue(variableSymbol, out var variable)
                ? variable : null;
        }

        public void ComputeCurrentObjectAccess()
        {
            var heapPlaces = AllHeapPlaces().ToFixedList();

            // Reset mutability to unknown (null)
            foreach (var place in heapPlaces)
                place.ResetAccess();

            var rootPlaces = callerVariables.Values.SafeCast<StackPlace>()
                                            .Concat(variables.Values)
                                            .Concat(tempValues)
                                            .ToFixedList();

            foreach (var place in rootPlaces)
                place.MarkReferencedObjects();
        }

        private IEnumerable<HeapPlace> AllHeapPlaces()
        {
            return contextObjects.Values.SafeCast<HeapPlace>().Concat(objects.Values);
        }

        /// <summary>
        /// Convert the graph into DOT format supported by graphviz
        /// </summary>
        public string ToDotFormat()
        {
            var dot = new StringBuilder();
            var nodes = new Dictionary<MemoryPlace, string>();
            dot.AppendLine("digraph reachability {");
            AppendStack(dot, nodes);
            AppendObjects(dot, nodes);
            AppendReferences(dot, nodes);
            dot.AppendLine("}");
            return dot.ToString();
        }

        private void AppendStack(StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            dot.AppendLine("    node [shape=plaintext];");
            dot.AppendLine("	stack [label=<");
            dot.AppendLine("        <TABLE BORDER=\"0\" CELLBORDER=\"1\" CELLSPACING=\"0\" CELLPADDING=\"4\">");
            var nextNode = 1;
            foreach (var callerVariable in CallerVariables)
                AppendStackItem(dot, callerVariable, "cv", nodes, ref nextNode);

            if (CallerVariables.Any() && Variables.Any())
                dot.AppendLine("            <TR><TD CELLPADDING=\"2\"></TD></TR>");

            nextNode = 1;
            foreach (var variable in Variables)
                AppendStackItem(dot, variable, "v", nodes, ref nextNode);

            dot.AppendLine("		</TABLE>>];"); // end stack
        }

        private static void AppendStackItem(
            StringBuilder dot,
            StackPlace place,
            string nodeType,
            Dictionary<MemoryPlace, string> nodes,
            ref int nextNode)
        {
            var port = nodeType + nextNode;
            nodes.Add(place, "stack:" + port);
            dot.AppendLine($"            <TR><TD PORT=\"{port}\" ALIGN=\"LEFT\">{place}</TD></TR>");
            nextNode += 1;
        }

        private void AppendObjects(StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            dot.AppendLine("    node [shape=ellipse];");

            var nextObject = 1;
            foreach (var contextObject in ContextObjects)
            {
                var name = "ctx" + nextObject;
                nodes.Add(contextObject, name);
                dot.AppendLine($"    {name} [label=\"{Escape(contextObject.ToString())}\"];");
                nextObject += 1;
            }

            nextObject = 1;
            foreach (var obj in Objects)
            {
                var name = "obj" + nextObject;
                nodes.Add(obj, name);
                dot.AppendLine($"    {name} [label=\"{Escape(obj.ToString())}\"];");
                nextObject += 1;
            }
        }

        private void AppendReferences(StringBuilder dot, Dictionary<MemoryPlace, string> nodes)
        {
            dot.AppendLine("    edge;");
            var sources = CallerVariables.Concat<MemoryPlace>(Variables).Concat(ContextObjects).Concat(Objects);
            foreach (var sourceNode in sources)
            {
                foreach (var reference in sourceNode.References)
                {
                    dot.Append($"    {nodes[sourceNode]} -> {nodes[reference.Referent]} [");
                    switch (reference.DeclaredAccess)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(reference.DeclaredAccess);
                        case Access.Identify:
                            dot.Append("[style=\"dotted\"]");
                            break;
                        case Access.Mutable:
                            dot.Append("[penwidth=\"3\"]");
                            break;
                        case Access.ReadOnly:
                            // Standard
                            break;
                    }
                    dot.AppendLine("];");
                }
            }
        }

        private static string Escape(string label)
        {
            return label.Replace("⟦", "&laquo;", StringComparison.InvariantCulture)
                        .Replace("⟧", "&raquo;", StringComparison.InvariantCulture);
        }
    }
}
