using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    public abstract class SymbolTestFixture
    {
        private int unique;

        protected NamespaceSymbol Namespace(string? name = null, NamespaceOrPackageSymbol? ns = null)
        {
            return new NamespaceSymbol(ns ?? Package(), Name(name) ?? DefaultName("namespace"));
        }

        protected PackageSymbol Package(string? name = null)
        {
            return new PackageSymbol(Name(name) ?? DefaultName("package"));
        }

        protected Name DefaultName(string prefix)
        {
            return new Name($"⧫{prefix}_{++unique}");
        }

        [return: NotNullIfNotNull("name")]
        protected static Name? Name(string? name = null)
        {
            return name is null ? null : new Name(name);
        }

        protected FixedList<DataType> Params(int? count = null)
        {
            return Enumerable.Range(1, count ?? ++unique).Select(n => Types.DataType.Int).ToFixedList<DataType>();
        }

        protected static FixedList<DataType> Params(DataType param, params DataType[] @params)
        {
            return @params.Prepend(param).ToFixedList();
        }

        protected FunctionSymbol Func(string? name = null, NamespaceOrPackageSymbol? ns = null, FixedList<DataType>? @params = null, DataType? @return = null)
        {
            return new FunctionSymbol(
                ns ?? Namespace(),
                Name(name) ?? DefaultName("func"),
                @params ?? Params(),
                @return ?? DataType());
        }

        protected static FunctionSymbol Func(
            FunctionSymbol mother,
            string? name = null,
            NamespaceOrPackageSymbol? ns = null,
            FixedList<DataType>? @params = null,
            DataType? @return = null)
        {
            return new FunctionSymbol(
                ns ?? mother.ContainingSymbol,
                Name(name) ?? mother.Name,
                @params ?? mother.ParameterDataTypes,
                @return ?? mother.ReturnDataType);
        }

        protected MethodSymbol Method(
            string? name = null,
            ObjectTypeSymbol? containing = null,
            DataType? self = null,
            FixedList<DataType>? @params = null,
            DataType? @return = null)
        {
            containing ??= Type();
            return new MethodSymbol(
                containing,
                Name(name) ?? DefaultName("method"),
                self ?? containing.DeclaresDataType,
                @params ?? Params(),
                @return ?? DataType());
        }

        protected static MethodSymbol Method(
            MethodSymbol mother,
            string? name = null,
            ObjectTypeSymbol? containing = null,
            DataType? self = null,
            FixedList<DataType>? @params = null,
            DataType? @return = null)
        {
            return new MethodSymbol(
                containing ?? mother.ContainingSymbol,
                Name(name) ?? mother.Name,
                self ?? mother.SelfDataType,
                @params ?? mother.ParameterDataTypes,
                @return ?? mother.ReturnDataType);
        }
        protected ObjectType DataType(string? name = null)
        {
            var finalName = Name(name) ?? DefaultName("DataType");
            return new ObjectType(NamespaceName.Global, finalName.Text, false, ReferenceCapability.Isolated);
        }

        protected ObjectTypeSymbol Type(
            NamespaceOrPackageSymbol? ns = null,
            ObjectType? dataType = null)
        {
            return new ObjectTypeSymbol(
                ns ?? Package(),
                dataType ?? DataType());
        }

        protected VariableSymbol Variable(
            string? name = null,
            InvocableSymbol? containing = null,
            int? declaration = null,
            bool? mut = null,
            DataType? type = null)
        {
            return new VariableSymbol(
                containing ?? Func(),
                Name(name) ?? DefaultName("variable"),
                declaration ?? ++unique,
                mut ?? true,
                type ?? DataType());
        }

        protected static VariableSymbol Variable(
            VariableSymbol mother,
            string? name = null,
            InvocableSymbol? containing = null,
            int? declaration = null,
            bool? mut = null,
            DataType? type = null)
        {
            return new VariableSymbol(
                containing ?? mother.ContainingSymbol,
                Name(name) ?? mother.Name,
                declaration ?? mother.DeclarationNumber,
                mut ?? mother.IsMutableBinding,
                type ?? mother.DataType);
        }
    }
}
