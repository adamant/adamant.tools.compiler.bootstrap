using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class DeclarationEmitter : IEmitter<DeclarationIL>
    {
        private readonly NameMangler nameMangler;
        private readonly IConverter<ParameterIL> parameterConverter;
        private readonly IConverter<DataType> typeConverter;
        private readonly IEmitter<IInvocableDeclarationIL> controlFlowEmitter;

        public DeclarationEmitter(
            NameMangler nameMangler,
            IConverter<ParameterIL> parameterConverter,
            IConverter<DataType> typeConverter,
            IEmitter<IInvocableDeclarationIL> controlFlowEmitter)
        {
            this.nameMangler = nameMangler;
            this.parameterConverter = parameterConverter;
            this.typeConverter = typeConverter;
            this.controlFlowEmitter = controlFlowEmitter;
        }

        public void Emit(DeclarationIL declaration, Code code)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case FunctionIL function:
                    if (function.IsExternal)
                        EmitExternalFunctionSignature(function, code);
                    else
                        EmitFunction(function, code);
                    break;
                case MethodDeclarationIL method:
                    EmitMethod(method, code);
                    break;
                case ConstructorIL constructor:
                    EmitConstructor(constructor, code);
                    break;
                case ClassIL type:
                    EmitType(type, code);
                    break;
                case FieldIL _:
                    // fields are emitted as part of the type
                    break;
            }
        }

        private void EmitExternalFunctionSignature(FunctionIL function, Code code)
        {
            // I think external functions are supposed to not be mangled?
            var name = function.Symbol.Name.Text;
            var parameters = Convert(function.Parameters);
            var returnType = typeConverter.Convert(function.Symbol.ReturnDataType.Known());
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");
        }

        private void EmitFunction(FunctionIL function, Code code)
        {
            var name = nameMangler.MangleName(function);
            var parameters = Convert(function.Parameters);
            var returnType = typeConverter.Convert(function.Symbol.ReturnDataType.Known());

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            code.Definitions.BeginBlock();
            controlFlowEmitter.Emit(function, code);
            code.Definitions.EndBlock();
        }

        private void EmitMethod(MethodDeclarationIL method, Code code)
        {
            if (method.IL is null) return;

            var name = nameMangler.MangleName(method);
            var parameters = Convert(method.Parameters.Prepend(method.SelfParameter));
            var returnType = typeConverter.Convert(method.Symbol.ReturnDataType.Known());

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            code.Definitions.BeginBlock();
            controlFlowEmitter.Emit(method, code);
            code.Definitions.EndBlock();
        }

        private void EmitConstructor(ConstructorIL constructor, Code code)
        {
            // Don't emit constructors without control flow, they are generic
            // TODO need to handle better
            if (constructor.IL is null) return;

            var name = nameMangler.MangleName(constructor);
            var parameters = Convert(constructor.Parameters);
            var returnType = typeConverter.Convert(constructor.Symbol.ContainingSymbol.DeclaresDataType.Known());

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"// {constructor.Symbol}");
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"// {constructor.Symbol}");
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            code.Definitions.BeginBlock();
            controlFlowEmitter.Emit(constructor, code);
            code.Definitions.EndBlock();
        }

        private string Convert(IEnumerable<ParameterIL> parameters)
        {
            return string.Join(", ", parameters.Select(parameterConverter.Convert));
        }

        private void EmitType(ClassIL @class, Code code)
        {
            var typeName = nameMangler.MangleName(@class);

            // Struct Forward Declarations
            var selfType = $"{typeName}___Self";
            var vtableType = $"{typeName}___VTable";
            var types = code.TypeDeclarations;
            types.AppendLine($"typedef struct {selfType} {selfType};");
            types.AppendLine($"typedef struct {vtableType} {vtableType};");
            // Declare the full type because when it is used as a field, its
            // size will need to be known. However, order within struct
            // declarations is not defined.
            types.AppendLine($"typedef struct {typeName}");
            types.BeginBlock();
            types.AppendLine($"{vtableType} const* restrict _vtable;");
            types.AppendLine($"{selfType}* restrict _self;");
            types.EndBlockWith($"}} {typeName};");

            var structs = code.StructDeclarations;
            structs.AppendLine($"struct {selfType}");
            structs.BeginBlock();
            foreach (var field in @class.Members.OfType<FieldIL>())
            {
                var binding = field.Symbol.IsMutableBinding ? "var" : "let";
                structs.AppendLine($"// {binding} {field.Symbol.Name}: {field.DataType}");
                var fieldType = typeConverter.Convert(field.DataType.Known());
                var fieldName = nameMangler.MangleName(field);
                structs.AppendLine($"{fieldType} {fieldName};");
            }
            structs.EndBlockWithSemicolon();
            structs.AppendLine($"struct {vtableType}");
            structs.BeginBlock();
            foreach (var method in @class.Members.OfType<MethodDeclarationIL>())
            {
                var name = nameMangler.MangleMethodName(method);
                var parameters = Convert(method.Parameters.Prepend(method.SelfParameter));
                var returnType = typeConverter.Convert(method.Symbol.ReturnDataType.Known());
                structs.AppendLine($"{returnType} (*{name})({parameters});");
            }
            structs.EndBlockWithSemicolon();

            var globals = code.StructDeclarations;
            globals.AppendLine($"const {vtableType} {typeName}___vtable = ({vtableType})");
            globals.BeginBlock();
            foreach (var method in @class.Members.OfType<MethodDeclarationIL>())
            {
                var fieldName = nameMangler.MangleMethodName(method);
                var functionName = nameMangler.MangleName(method);
                globals.AppendLine($".{fieldName} = {functionName},");
            }
            globals.EndBlockWithSemicolon();
        }
    }
}
