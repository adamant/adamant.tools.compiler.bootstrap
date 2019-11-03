using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class DeclarationEmitter : IEmitter<Declaration>
    {
        private readonly NameMangler nameMangler;
        private readonly IConverter<Parameter> parameterConverter;
        private readonly IConverter<DataType> typeConverter;
        private readonly IEmitter<ControlFlowGraph> controlFlowEmitter;

        public DeclarationEmitter(
            NameMangler nameMangler,
            IConverter<Parameter> parameterConverter,
            IConverter<DataType> typeConverter,
            IEmitter<ControlFlowGraph> controlFlowEmitter)
        {
            this.nameMangler = nameMangler;
            this.parameterConverter = parameterConverter;
            this.typeConverter = typeConverter;
            this.controlFlowEmitter = controlFlowEmitter;
        }

        public void Emit(Declaration declaration, Code code)
        {
            switch (declaration)
            {
                case FunctionDeclaration function:
                    if (function.IsExternal)
                        EmitExternalFunctionSignature(function, code);
                    else
                        EmitFunction(function, code);
                    break;
                case ConstructorDeclaration constructor:
                    EmitConstructor(constructor, code);
                    break;
                case ClassDeclaration type:
                    EmitType(type, code);
                    break;
                case FieldDeclaration _:
                    // fields are emitted as part of the type
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void EmitExternalFunctionSignature(FunctionDeclaration function, Code code)
        {
            var name = function.Name.Text;
            var parameters = Convert(function.Parameters);
            var returnType = typeConverter.Convert(function.ReturnType.AssertKnown());
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");
        }

        private void EmitFunction(FunctionDeclaration function, Code code)
        {
            // Don't emit functions without control flow, they are generic
            // TODO need to handle better
            if (function.ControlFlow == null) return;

            var name = nameMangler.MangleName(function);
            var parameters = Convert(function.Parameters);
            var returnType = typeConverter.Convert(function.ReturnType.AssertKnown());

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            code.Definitions.BeginBlock();
            controlFlowEmitter.Emit(function.ControlFlow, code);
            code.Definitions.EndBlock();
        }

        private void EmitConstructor(ConstructorDeclaration constructor, Code code)
        {
            // Don't emit constructors without control flow, they are generic
            // TODO need to handle better
            if (constructor.ControlFlow == null) return;

            var name = nameMangler.MangleName(constructor);
            var parameters = Convert(constructor.Parameters);
            var returnType = typeConverter.Convert(constructor.ReturnType.AssertKnown());

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            code.Definitions.BeginBlock();
            controlFlowEmitter.Emit(constructor.ControlFlow, code);
            code.Definitions.EndBlock();
        }

        private string Convert(IEnumerable<Parameter> parameters)
        {
            return string.Join(", ", parameters.Select(parameterConverter.Convert));
        }

        private void EmitType(ClassDeclaration @class, Code code)
        {
            var typeName = nameMangler.MangleName(@class);

            // Struct Forward Declarations
            var selfType = $"{typeName}___Self";
            var vtableType = $"{typeName}___VTable";
            var types = code.TypeDeclarations;
            types.AppendLine($"typedef struct {selfType} {selfType};");
            types.AppendLine($"typedef struct {vtableType} {vtableType};");
            types.AppendLine($"typedef struct {typeName} {typeName};");

            var structs = code.StructDeclarations;
            structs.AppendLine($"struct {selfType}");
            structs.BeginBlock();
            foreach (var field in @class.Members.OfType<FieldDeclaration>())
            {
                var fieldType = typeConverter.Convert(field.Type.AssertKnown());
                var fieldName = nameMangler.Mangle(field.Name);
                structs.AppendLine($"{fieldType} {fieldName};");
            }
            structs.EndBlockWithSemicolon();
            structs.AppendLine($"struct {vtableType}");
            structs.BeginBlock();
            foreach (var function in @class.Members.OfType<FunctionDeclaration>())
            {
                var name = nameMangler.MangleUnqualifiedName(function);
                var parameters = Convert(function.Parameters);
                var returnType = typeConverter.Convert(function.ReturnType.AssertKnown());
                structs.AppendLine($"{returnType} (*{name})({parameters});");
            }
            structs.EndBlockWithSemicolon();

            var globals = code.StructDeclarations;
            globals.AppendLine($"const {vtableType} {typeName}___vtable = ({vtableType})");
            globals.BeginBlock();
            foreach (var function in @class.Members.OfType<FunctionDeclaration>())
            {
                var fieldName = nameMangler.MangleUnqualifiedName(function);
                var functionName = nameMangler.MangleName(function);
                globals.AppendLine($".{fieldName} = {functionName},");
            }
            globals.EndBlockWithSemicolon();

            structs.AppendLine($"struct {typeName}");
            structs.BeginBlock();
            structs.AppendLine($"{vtableType} const* restrict _vtable;");
            structs.AppendLine($"{selfType}* restrict _self;");
            structs.EndBlockWithSemicolon();
        }
    }
}
