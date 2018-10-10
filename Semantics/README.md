# Adamant.Tools.Compiler.Bootstrap.Semantics

There have been several approaches to the semantic analysis portion of the compiler.

## Original Approach

At first, it was thought that this compiler should be designed to plan for reimplementation in Adamant. Doing so imposed a number of constraints:

1. Avoid Reference Cycles - borrow checking and reference counting in Adamant make reference cycles problematic. In particular, there was concern about links across the tree. To avoid those, the plan had been to store them as name or data type objects that could be used to look up the relevant nodes in a symbol tree.
2. Incremental Compilation - prevented the use of references to parent nodes even though these might be ok in Adamant because it was expected nodes would be shared between different versions of the tree. Also, it pushed for immutable data structures for all phases of the compiler. It was not planned that this compiler would actually implement incremental compilation though.
3. Arbitrary Compile Time Code Execution - means that there could be arbitrarily complex connections between which parts of the semantic tree must be evaluated before other parts. For example, the ability to declare the return type of a function to be the result of a meta-function and the ability of pure functions to be used in constant and generic argument contexts means that some functions will have to be fully analyzed and interpreted before other functions can even be typed.
4. Parallel Compilation - it must be possible to break the work into chunks while not producing undo contention.
5. Strong Typing - to avoid bugs involving missing values or values of incorrect types.

Trying to support all of these while creating a compiler that could be rapidly developed and changed was challenging. C# seemed to make everything very verbose. A series of approaches were tried. Eventually a system allowing for a fairly direct expression of attribute grammars over the syntax tree was adopted. However, this makes strong typing challenging. Worse than that, because everything is computed on demand it makes order of execution and debugging very confusing. To simplify the implementation of the borrow checker an intermediate language (IL) was introduced. This was helpful, however, it was added as a later phase to semantic analysis which then became an issue because it couldn't be used for compile time code execution because it could only be generated after all semantic analysis was complete.

## Current Plan

Of the above constraints, all but arbitrary compile time code execution and strong typing have been dropped. It is hoped this will make development quicker, easier and more testable. To make things clear and easy, the plan to to have a series of clearly defined steps. Of the below list, only steps 2 through 4 are part of semantic the semantic analysis in this project. Steps 1 and 5 are included for context.

1. Lexing and Parsing produce a concrete syntax tree. Each compilation unit can be done in parallel.
2. Build a simplified tree of declarations and members for the package. (Consider including simplified statements and expressions too)
3. Build lexical scopes (i.e. name tables) for name resolution. Each compilation unit can be done in parallel.
4. Analyze the semantics. This must mix name binding, type checking, IL generation and compile time code execution because they are interdependent. To simplify this as much as possible, this will be single threaded and will be free to mutate the tree as needed. All mutator method/properties will be `internal` to other phases can't mutate the tree. The `Lazy<T>` type may or may not be used to make cycle detection easier.
5. Emit C code from the generated IL.

### Semantic/IL Tree

The current plan is that the tree generated for the semantic analysis and IL generation would be something roughly consistent with what could be used in an IL representation of an Adamant package. However, it may include additional data or optional data structures that wouldn't be used if parsing and generating IL. In order to try to keep this structure as simple as possible, it is planned the following abstractions relative to the concrete syntax trees will be made:

* Unify partial classes
* Don't represent namespaces hierarchically, each declaration is fully qualified
* Don't include parenthesized expressions
* Unify the different kinds of blocks
* All loops as `loop`?
