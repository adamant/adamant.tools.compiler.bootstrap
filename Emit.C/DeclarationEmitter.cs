using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class DeclarationEmitter : IEmitter<Declaration>
    {
        [NotNull] private readonly NameMangler nameMangler;
        [NotNull] private readonly IConverter<Parameter> parameterConverter;
        [NotNull] private readonly IConverter<DataType> typeConverter;
        [NotNull] private readonly IEmitter<ControlFlowGraph> controlFlowEmitter;

        public DeclarationEmitter(
            [NotNull] NameMangler nameMangler,
            [NotNull] IConverter<Parameter> parameterConverter,
            [NotNull] IConverter<DataType> typeConverter,
            [NotNull] IEmitter<ControlFlowGraph> controlFlowEmitter)
        {
            Requires.NotNull(nameof(nameMangler), nameMangler);
            Requires.NotNull(nameof(parameterConverter), parameterConverter);
            Requires.NotNull(nameof(typeConverter), typeConverter);
            Requires.NotNull(nameof(controlFlowEmitter), controlFlowEmitter);
            this.nameMangler = nameMangler;
            this.parameterConverter = parameterConverter;
            this.typeConverter = typeConverter;
            this.controlFlowEmitter = controlFlowEmitter;
        }

        public void Emit([NotNull] Declaration declaration, [NotNull] Code code)
        {
            switch (declaration)
            {
                case FunctionDeclaration function:
                    EmitFunction(function, code);
                    break;
                case ConstructorDeclaration constructor:
                    EmitConstructor(constructor, code);
                    break;
                case TypeDeclaration type:
                    EmitType(type, code);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void EmitFunction([NotNull] FunctionDeclaration function, [NotNull] Code code)
        {
            // Don't emit functions without control flow, they are generic
            // TODO need to handle better
            if (function.ControlFlow == null) return;

            var name = nameMangler.MangleName(function);
            var parameters = Convert(function.Parameters);
            var returnType = typeConverter.Convert(function.ReturnType.AssertResolved());

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            code.Definitions.BeginBlock();
            controlFlowEmitter.Emit(function.ControlFlow, code);
            code.Definitions.EndBlock();
        }

        private void EmitConstructor([NotNull] ConstructorDeclaration constructor, [NotNull] Code code)
        {
            // Don't emit constructors without control flow, they are generic
            // TODO need to handle better
            if (constructor.ControlFlow == null) return;

            var name = nameMangler.MangleName(constructor);
            var parameters = Convert(constructor.Parameters);
            var returnType = typeConverter.Convert(constructor.ReturnType.AssertResolved());

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            code.Definitions.BeginBlock();
            controlFlowEmitter.Emit(constructor.ControlFlow, code);
            code.Definitions.EndBlock();
        }

        private string Convert([NotNull][ItemNotNull] IEnumerable<Parameter> parameters)
        {
            return string.Join(", ", parameters.Select(parameterConverter.Convert));
        }

        private void EmitType([NotNull] TypeDeclaration type, [NotNull] Code code)
        {
            var typeName = nameMangler.MangleName(type);

            // Struct Declarations
            var selfType = $"{typeName}·ₐSelf";
            var vtableType = $"{typeName}·ₐV_Table";
            code.TypeDeclarations.AppendLine($"typedef struct {selfType} {selfType};");
            code.TypeDeclarations.AppendLine($"typedef struct {vtableType} {vtableType};");
            code.TypeDeclarations.AppendLine($"typedef struct {{ {vtableType} const*_Nonnull restrict ₐvtable; {selfType} const*_Nonnull restrict ₐself; }} {typeName};");
            code.TypeDeclarations.AppendLine($"typedef struct {{ {vtableType} const*_Nonnull restrict ₐvtable; {selfType} *_Nonnull restrict ₐself; }} mut˽{typeName};");

            code.StructDeclarations.AppendLine($"struct {selfType}");
            code.StructDeclarations.BeginBlock();
            code.StructDeclarations.EndBlockWithSemicolon();
        }
    }
}
