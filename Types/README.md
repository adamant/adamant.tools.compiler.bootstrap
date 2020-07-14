# Adamant.Tools.Compiler.Bootstrap.Types

Types name the possible types in a program. Many types correspond directly to some name in the program. However, other types are constructed out of those types. For example, by substituting in type arguments. Types may be either *open* or *closed*. An open type is one which has at least one unspecific type parameter. A closed type has all of its type parameters fully specified.

## Constant Types and Values

In the Adamant compiler, constant values of expressions are expressed as special types. There are several reasons for this:

1. It ensures strong typing by coupling the constant value to the type rather than keeping a separate constant value for an expression which must then be cast to the correct type based on the type of the expression.
2. Integer constants must have their own type anyway because they have arbitrary size.
3. It is in line with the planned ability to have values as generic parameters. Thus parameterized types on values. It may one day make sense to expose this in the language. That is `bool[true]` could be the type of a boolean known to be true at compile time. Likewise, `int[0]` the type of an integer known to be zero at compile time. One possible use for this is in a units of measure library where `m^3` could be handled by overloading the `^` operator on `int[1]`, `int[2]`, `int[3]` etc.
4. It makes supporting operations like supporting concatenation of constant strings very easy.
