using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class DeclarationEmitter : IEmitter<Declaration>
    {
        private readonly NameMangler nameMangler;
        private readonly IConverter<Parameter> parameterConverter;
        private readonly IConverter<DataType> typeConverter;
        private readonly IEmitter<ICallableDeclaration> controlFlowEmitter;

        public DeclarationEmitter(
            NameMangler nameMangler,
            IConverter<Parameter> parameterConverter,
            IConverter<DataType> typeConverter,
            IEmitter<ICallableDeclaration> controlFlowEmitter)
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
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case FunctionDeclaration function:
                    if (function.IsExternal)
                        EmitExternalFunctionSignature(function, code);
                    else
                        EmitFunction(function, code);
                    break;
                case MethodDeclaration method:
                    EmitMethod(method, code);
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
            }
        }

        private void EmitExternalFunctionSignature(FunctionDeclaration function, Code code)
        {
            var name = function.Name.Text;
            var parameters = Convert(function.Parameters);
            var returnType = typeConverter.Convert(function.ReturnType.Known());
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");
        }

        private void EmitFunction(FunctionDeclaration function, Code code)
        {
            var name = nameMangler.MangleName(function);
            var parameters = Convert(function.Parameters);
            var returnType = typeConverter.Convert(function.ReturnType.Known());

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            code.Definitions.BeginBlock();
            controlFlowEmitter.Emit(function, code);
            code.Definitions.EndBlock();
        }

        private void EmitMethod(MethodDeclaration method, Code code)
        {
            if (method.IL is null) return;

            var name = nameMangler.MangleName(method);
            var parameters = Convert(method.Parameters.Prepend(method.SelfParameter));
            var returnType = typeConverter.Convert(method.ReturnType.Known());

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            code.Definitions.BeginBlock();
            controlFlowEmitter.Emit(method, code);
            code.Definitions.EndBlock();
        }

        private void EmitConstructor(ConstructorDeclaration constructor, Code code)
        {
            // Don't emit constructors without control flow, they are generic
            // TODO need to handle better
            if (constructor.IL is null) return;

            var name = nameMangler.MangleName(constructor);
            var parameters = Convert(constructor.Parameters);
            var returnType = typeConverter.Convert(constructor.ReturnType.Known());

            // Write out the function declaration for C so we can call functions defined after others
            code.FunctionDeclarations.AppendLine($"{returnType} {name}({parameters});");

            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine($"{returnType} {name}({parameters})");
            code.Definitions.BeginBlock();
            controlFlowEmitter.Emit(constructor, code);
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
                var fieldType = typeConverter.Convert(field.Type.Known());
                var fieldName = nameMangler.Mangle(field.Name);
                structs.AppendLine($"{fieldType} {fieldName};");
            }
            structs.EndBlockWithSemicolon();
            structs.AppendLine($"struct {vtableType}");
            structs.BeginBlock();
            foreach (var method in @class.Members.OfType<MethodDeclaration>())
            {
                var name = nameMangler.MangleUnqualifiedName(method);
                var parameters = Convert(method.Parameters.Prepend(method.SelfParameter));
                var returnType = typeConverter.Convert(method.ReturnType.Known());
                structs.AppendLine($"{returnType} (*{name})({parameters});");
            }
            structs.EndBlockWithSemicolon();

            var globals = code.StructDeclarations;
            globals.AppendLine($"const {vtableType} {typeName}___vtable = ({vtableType})");
            globals.BeginBlock();
            foreach (var method in @class.Members.OfType<MethodDeclaration>())
            {
                var fieldName = nameMangler.MangleUnqualifiedName(method);
                var functionName = nameMangler.MangleName(method);
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
