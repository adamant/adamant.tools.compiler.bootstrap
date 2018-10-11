using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class DeclarationEmitter : IEmitter<Declaration>
    {
        [NotNull] private readonly NameMangler nameMangler;
        [NotNull] private readonly IConverter<Parameter> parameterConverter;
        [NotNull] private readonly IConverter<DataType> typeConverter;

        public DeclarationEmitter(
            [NotNull] NameMangler nameMangler,
            [NotNull] IConverter<Parameter> parameterConverter,
            [NotNull] IConverter<DataType> typeConverter)
        {
            this.nameMangler = nameMangler;
            this.parameterConverter = parameterConverter;
            this.typeConverter = typeConverter;
        }

        public void Emit([NotNull] Declaration declaration, [NotNull]  Code code)
        {
            switch (declaration)
            {
                case FunctionDeclaration function:
                    Emit(function, code);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void Emit([NotNull] FunctionDeclaration function, [NotNull] Code code)
        {
            var name = nameMangler.MangleName(function);
            var parameters = Convert(function.Parameters);
            var returnType = typeConverter.Convert(function.ReturnType);

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            //Emit(code, function.Body);
            code.Definitions.BeginBlock();
            code.Definitions.EndBlock();
        }

        private string Convert([NotNull][ItemNotNull] IEnumerable<Parameter> parameters)
        {
            return string.Join(", ", parameters.Select(parameterConverter.Convert));
        }
    }
}
