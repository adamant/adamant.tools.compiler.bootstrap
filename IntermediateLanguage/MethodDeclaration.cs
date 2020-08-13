using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class MethodDeclaration : Declaration, IInvocableDeclaration, IMethodMetadata
    {
        public bool IsExternal => false;
        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types",
            Justification = "NA")]
        bool IInvocableDeclaration.IsConstructor => false;
        public Parameter SelfParameter { get; }

        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types",
            Justification = "NA")]
        IBindingMetadata IMethodMetadata.SelfParameterMetadata => SelfParameter;
        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public DataType ReturnDataType { get; }
        public ControlFlowGraph? IL { get; }

        IEnumerable<IBindingMetadata> IFunctionMetadata.Parameters => Parameters;

        public MethodDeclaration(
            MaybeQualifiedName name,
            Parameter selfParameter,
            FixedList<Parameter> parameters,
            DataType returnType,
            ControlFlowGraph? il)
            : base(true, name, MetadataSet.Empty)
        {
            SelfParameter = selfParameter;
            Parameters = parameters;
            ReturnDataType = returnType;
            IL = il;
        }
    }
}
