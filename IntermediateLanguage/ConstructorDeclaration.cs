using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ConstructorDeclaration : Declaration, ICallableDeclaration, IFunctionMetadata
    {
        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "NA")]
        bool ICallableDeclaration.IsExternal => false;

        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "NA")]
        bool ICallableDeclaration.IsConstructor => true;
        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public DataType ReturnDataType { get; }
        public FixedList<FieldInitialization> FieldInitializations { get; }
        public ControlFlowGraph IL { get; }

        IEnumerable<IBindingMetadata> IFunctionMetadata.Parameters => Parameters;

        public ConstructorDeclaration(
            Name fullName,
            FixedList<Parameter> parameters,
            DataType returnType,
            FixedList<FieldInitialization> fieldInitializations,
            ControlFlowGraph il)
            : base(true, fullName, MetadataSet.Empty)
        {
            ReturnDataType = returnType;
            IL = il;
            FieldInitializations = fieldInitializations;
            Parameters = parameters;
        }
    }
}
