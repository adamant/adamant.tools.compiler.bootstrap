using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;
using VariableDeclaration = Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.VariableDeclaration;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class ControlFlowEmitter : IEmitter<ControlFlowGraph>
    {
        private readonly NameMangler nameMangler;
        private readonly IConverter<DataType> typeConverter;

        public ControlFlowEmitter(
            NameMangler nameMangler,
            IConverter<DataType> typeConverter)
        {
            this.typeConverter = typeConverter;
            this.nameMangler = nameMangler;
        }

        public void Emit(ControlFlowGraph cfg, Code code)
        {
            var definitions = code.Definitions;

            foreach (var declaration in cfg.VariableDeclarations.Where(v => v.TypeIsNotEmpty))
                EmitVariable(declaration, definitions);

            if (cfg.VariableDeclarations.Any(v => v.TypeIsNotEmpty))
                definitions.BlankLine();

            foreach (var block in cfg.Blocks)
                EmitBlock(block, definitions);
        }

        private void EmitVariable(VariableDeclaration declaration, CCodeBuilder code)
        {
            Requires.That(nameof(declaration), declaration.TypeIsNotEmpty, "tried to look up variable that does not exist");
            var initializer = declaration.IsParameter ? $" = {nameMangler.Mangle(declaration.Name!)}" : "";
            code.AppendLine($"{typeConverter.Convert(declaration.Type)} _{declaration.Variable.Name}{initializer}; // {declaration}");
        }

        private void EmitBlock(Block block, CCodeBuilder code)
        {
            code.AppendLine($"bb{block.Number}:");
            code.BeginBlock();
            foreach (var instruction in block.Instructions)
            {
                EmitStatement(instruction, code);
            }
            code.EndBlock();
        }

        private static void EmitStatement(Instruction instruction, CCodeBuilder code)
        {
            code.AppendLine("// " + instruction);
            switch (instruction)
            {
                default:
                    throw new NotImplementedException();
                    //throw ExhaustiveMatch.Failed(instruction);

                    //case IfStatement ifStatement:
                    //    code.AppendLine($"if({ConvertValue(ifStatement.Condition)}._value) goto {ifStatement.ThenBlock}; else goto {ifStatement.ElseBlock};");
                    //    break;
                    //case GotoStatement gotoStatement:
                    //    code.AppendLine($"goto {gotoStatement.GotoBlock};");
                    //    break;
                    //case ReturnStatement _:
                    //    code.AppendLine(voidReturn ? "return;" : "return _result;");
                    //    break;
                    //case AssignmentStatement assignment:
                    //    code.AppendLine(
                    //        $"{ConvertPlace(assignment.Place)} = {ConvertValue(assignment.Value)};");
                    //    break;
                    //case ActionStatement action:
                    //    code.AppendLine(ConvertValue(action.Value) + ";");
                    //    break;
                    //case DeleteStatement deleteStatement:
                    //{
                    //    var self = ConvertValue(deleteStatement.Place);
                    //    var typeName = nameMangler.Mangle(deleteStatement.Type);
                    //    // TODO once deletes are implemented, call them
                    //    //code.AppendLine($"{self}._vtable->{typeName}___delete__1({self});");
                    //    code.AppendLine($"free({self}._self);");
                    //    break;
                    //}
                    //case ExitScopeStatement _:
                    //    // End scope isn't emitted, it is just a marker
                    //    break;
            }
        }

        private string ConvertPlace(Place place)
        {
            switch (place)
            {
                default:
                    throw ExhaustiveMatch.Failed(place);
                case VariablePlace reference:
                    return "_" + reference.Variable.Name;
                case FieldPlace fieldAccess:
                    var fieldName = nameMangler.Mangle(fieldAccess.Field.UnqualifiedName);
                    return $"{ConvertValue(fieldAccess.Target)}._self->{fieldName}";
            }
        }

        private static string ConvertValue(Operand value)
        {
            switch (value)
            {
                default:
                    throw new NotImplementedException();
                    //throw ExhaustiveMatch.Failed(value);

                    //case IntegerConstant integer:
                    //    return $"({ConvertType(integer.Type)}){{{integer.Value}}}";
                    //case StringConstant stringConstant:
                    //{
                    //    var constantLength = StringConstant.Encoding.GetByteCount(stringConstant.Value);
                    //    const string selfArgument = "(String){&String___vtable, malloc(sizeof(String___Self))}";
                    //    return $"String___new__3({selfArgument}, (_size){{{constantLength}}}, (_size){{(uintptr_t)u8\"{stringConstant.Value.Escape()}\"}})";
                    //}
                    //case BooleanConstant boolean:
                    //    var booleanValue = boolean.Value ? 1 : 0;
                    //    return $"({ConvertType(boolean.Type)}){{{booleanValue}}}";
                    //case FunctionCall functionCall:
                    //{
                    //    var mangledName = nameMangler.Mangle(functionCall.FunctionName);
                    //    var arguments = functionCall.Self.YieldValue().Concat(functionCall.Arguments).Select(ConvertValue);
                    //    return $"{mangledName}__{functionCall.Arity}({string.Join(", ", arguments)})";
                    //}
                    //case ConstructorCall constructorCall:
                    //{
                    //    var typeName = nameMangler.Mangle(constructorCall.Type);
                    //    var selfArgument = $"({typeName}){{&{typeName}___vtable, malloc(sizeof({typeName}___Self))}}";
                    //    var arguments = selfArgument.YieldValue().Concat(constructorCall.Arguments.Select(ConvertValue));
                    //    return $"{typeName}___new__{constructorCall.Arity}({string.Join(", ", arguments)})";
                    //}
                    //case DeclaredValue declaredValue:
                    //    return nameMangler.Mangle(declaredValue.Name);
                    //case FieldAccess fieldAccess:
                    //    var fieldName = nameMangler.Mangle(fieldAccess.Field.UnqualifiedName);
                    //    return $"{ConvertValue(fieldAccess.Expression)}._self->{fieldName}";
                    //case IPlace place:
                    //    return ConvertPlace(place);
                    //case VirtualFunctionCall virtualCall:
                    //{
                    //    var self = ConvertValue(virtualCall.Self);
                    //    var mangledName = nameMangler.Mangle(virtualCall.FunctionName);
                    //    var arity = virtualCall.Arguments.Count + 1;
                    //    var arguments = virtualCall.Arguments.Select(ConvertValue).Prepend(self);
                    //    return $"{self}._vtable->{mangledName}__{arity}({string.Join(", ", arguments)})";
                    //}
                    //case BinaryOperation binaryOperation:
                    //{
                    //    var left = ConvertValue(binaryOperation.LeftOperand);
                    //    var right = ConvertValue(binaryOperation.RightOperand);
                    //    var operationType = nameMangler.Mangle(binaryOperation.Type);
                    //    string @operator;
                    //    switch (binaryOperation.Operator)
                    //    {
                    //        default:
                    //            throw ExhaustiveMatch.Failed(binaryOperation.Operator);
                    //        // If a binary operator was emitted for a boolean operation,
                    //        // then it doesn't short circuit, we just call the function
                    //        case BinaryOperator.And:
                    //            @operator = "and";
                    //            break;
                    //        case BinaryOperator.Or:
                    //            @operator = "or";
                    //            break;
                    //        case BinaryOperator.Plus:
                    //            @operator = "add";
                    //            break;
                    //        case BinaryOperator.Minus:
                    //            @operator = "sub";
                    //            break;
                    //        case BinaryOperator.Asterisk:
                    //            @operator = "mul";
                    //            break;
                    //        case BinaryOperator.Slash:
                    //            @operator = "div";
                    //            break;
                    //        case BinaryOperator.EqualsEquals:
                    //            @operator = "eq";
                    //            break;
                    //        case BinaryOperator.NotEqual:
                    //            @operator = "ne";
                    //            break;
                    //        case BinaryOperator.LessThan:
                    //            @operator = "lt";
                    //            break;
                    //        case BinaryOperator.LessThanOrEqual:
                    //            @operator = "lte";
                    //            break;
                    //        case BinaryOperator.GreaterThan:
                    //            @operator = "gt";
                    //            break;
                    //        case BinaryOperator.GreaterThanOrEqual:
                    //            @operator = "gte";
                    //            break;
                    //        case BinaryOperator.DotDot:
                    //            @operator = "dotdot";
                    //            break;
                    //        case BinaryOperator.LessThanDotDot:
                    //            @operator = "ltdotdot";
                    //            break;
                    //        case BinaryOperator.DotDotLessThan:
                    //            @operator = "dotdotlt";
                    //            break;
                    //        case BinaryOperator.LessThanDotDotLessThan:
                    //            @operator = "ltdotdotlt";
                    //            break;
                    //    }
                    //    return $"{operationType}__{@operator}({left}, {right})";
                    //}
                    //case NoneConstant noneConstant:
                    //    switch (noneConstant.Type.Referent)
                    //    {
                    //        case UserObjectType userObjectType:
                    //        {
                    //            var typeName = nameMangler.Mangle(userObjectType);
                    //            return $"({noneConstant.Type}){{&{typeName}___vtable, NULL}}";
                    //        }

                    //        case SimpleType simpleType:
                    //        {
                    //            var typeName = nameMangler.Mangle(simpleType);
                    //            return $"_opt__{typeName}__none";
                    //        }

                    //        default:
                    //            throw NonExhaustiveMatchException.For(noneConstant.Type);
                    //    }

                    //case ConstructSome constructSome:
                    //{
                    //    var someValue = ConvertValue(constructSome.Value);
                    //    if (constructSome.Type.Referent is ReferenceType)
                    //        return someValue;
                    //    else
                    //    {
                    //        var typeName = nameMangler.Mangle(constructSome.Type.Referent);
                    //        return $"_opt__{typeName}__Some({someValue})";
                    //    }
                    //}
                    //case UnaryOperation _:
                    //    throw new NotImplementedException();
                    //case Conversion conversion:
                    //{
                    //    var valueToConvert = ConvertValue(conversion.Operand);
                    //    var fromType = nameMangler.Mangle(conversion.FromType);
                    //    var toType = nameMangler.Mangle(conversion.ToType);
                    //    return $"_convert__{fromType}__{toType}({valueToConvert})";
                    //}
            }
        }

        private string ConvertType(DataType type)
        {
            switch (type)
            {
                default:
                    throw new NotImplementedException();
                //throw ExhaustiveMatch.Failed(type);
                case UserObjectType objectType:
                    return nameMangler.Mangle(objectType);
                case SimpleType simpleType:
                    return nameMangler.Mangle(simpleType.Name);
            }
        }
    }
}
