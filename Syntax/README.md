# Adamant.Tools.Compiler.Bootstrap.Syntax

The current design of the lexing, parsing and concrete syntax tree portions of the compiler is the result of a number of evolutions in the design.

Initially, it was hoped there could be some support for incremental compilation through reuse of unchanged portions of the syntax tree. That is the strategy taken by the Roslyn C# compiler. Eventually, it was decided that this was too complex. It is more important to get a compiler up and working.

## Design Goals

### Adaptability to Syntax Changes

As the language is still being designed, it is important that the lexer, parser and concrete syntax tree be easy to modify. This is supported by keeping them as simple as possible. Code generation is used around tokens so that tokens can be easily added and removed. Several points in the lexer use generated lists of all tokens of a given type to guide the lexing process.

### Correct by Construction

Use of the nullability attributes and `Requires` preconditions aid in ensuring the lexer and parser are correct. Originally, tokens were not as strongly typed. They were using several struct types with an enum for the token type. This performance optimization was modelled on the Roslyn C# compiler. However, it was decided that having strongly typed tokens was more valuable and the transition was made to the current set of classes. This also allowed for more sophisticated type relationships such as a base class for keywords and a base class for operators. Ideally, missing tokens would be represented with a special token that included the type and position of the missing token. However, the C# type system didn't offer good options for representing that in a strongly typed way. It was decided to use `null` to represent missing tokens and accept the limitations that might imply.

### Testable

Testability is an important goal. Given the constantly shifting language design and limited development resources, easy automated testing is particularly valuable. Originally, tests were to be described by configuration files that could be shared with any future compiler writers and a language test suite. These give an example of code along with the description of how it should be parsed or analyzed. These tests were very brittle and it was realized they were too highly coupled to the internal representations of the current compiler to be that useful for testing other compilers. Consequently, the testing strategy has been shifted to focus more on unit tests. These are less brittle because the focus on smaller parts and are changed by automated refactoring when code is changed. These are supported by use of the FsCheck library for property based testing. This has been challenging, but invaluable. The language test suite still exists, but has been changed to sample programs that need only be compiled or run. This decouples them from any compiler internals.
