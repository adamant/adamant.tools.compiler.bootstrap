using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class FunctionDeclaration : Declaration, ICallableDeclaration, IFunctionMetadata
    {
        public bool IsExternal { get; }

        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "NA")]
        bool ICallableDeclaration.IsConstructor => false;

        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public DataType ReturnDataType { get; }
        public ControlFlowGraph IL { get; }

        IEnumerable<IBindingMetadata> IFunctionMetadata.Parameters => Parameters;

        public FunctionDeclaration(
            bool isExternal,
            bool isMember,
            Name name,
            FixedList<Parameter> parameters,
            DataType returnType,
            ControlFlowGraph il)
            : base(isMember, name, MetadataSet.Empty)
        {
            Parameters = parameters;
            ReturnDataType = returnType;
            IL = il;
            IsExternal = isExternal;
        }
    }
}
