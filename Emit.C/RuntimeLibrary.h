#include <stdint.h>

// Rather than including all of stdlib.h we only declare the things we need.
void* malloc(size_t size);
void free(void* ptr);

// Cross platform way to mark a variable or parameter as unused to avoid warnings
#define _UNUSED(x) (void)(x)

#define OPTIONAL_TYPE(type) \
typedef struct { _Bool _hasValue; type _value; } _opt__##type; \
inline _opt__##type _opt__##type##__Some(type x) { return (_opt__##type) {1, x}; } \
extern const _opt__##type _opt__##type##__none;

// `bool` type
typedef struct { _Bool _value; } _bool;

inline _bool _bool__and(_bool x, _bool y) { return (_bool) { x._value& y._value }; }
inline _bool _bool__or(_bool x, _bool y) { return (_bool) { x._value | y._value }; }

OPTIONAL_TYPE(_bool)

// Integer Type Operations Macro
#define INTEGER_OPERATIONS(type) \
inline type type##__add(type x, type y) { return (type) { x._value + y._value }; } \
inline type type##__sub(type x, type y) { return (type) { x._value - y._value }; } \
inline type type##__mul(type x, type y) { return (type) { x._value* y._value }; } \
inline type type##__div(type x, type y) { return (type) { x._value / y._value }; } \
inline type type##__remainder__1(type x, type y) { return (type) { x._value% y._value }; } \
inline _bool type##__eq(type x, type y) { return (_bool) { x._value == y._value }; } \
inline _bool type##__ne(type x, type y) { return (_bool) { x._value != y._value }; } \
inline _bool type##__lt(type x, type y) { return (_bool) { x._value < y._value }; } \
inline _bool type##__lte(type x, type y) { return (_bool) { x._value <= y._value }; } \
inline _bool type##__gt(type x, type y) { return (_bool) { x._value > y._value }; } \
inline _bool type##__gte(type x, type y) { return (_bool) { x._value >= y._value }; }

// `int` type
typedef struct { int32_t _value; } _int;

INTEGER_OPERATIONS(_int)
OPTIONAL_TYPE(_int)

// `uint` type
typedef struct { uint32_t _value; } _uint;

INTEGER_OPERATIONS(_uint)

// `byte` type
typedef struct { uint8_t _value; } _byte;

INTEGER_OPERATIONS(_byte)

// `size` type
typedef struct { uintptr_t _value; } _size;

INTEGER_OPERATIONS(_size)

// `offset` type
typedef struct { intptr_t _value; } _offset;

INTEGER_OPERATIONS(_offset)

// `String` type
// Note: For now, strings are intrinsic
typedef struct
{
    _size byte_count;
    _byte* bytes;
} String;

inline String _String_new(size_t byte_count, uint8_t* bytes)
{
    return (String) {(_size){byte_count}, (_byte*)bytes };
}
_bool String___op_equals__2(String left, String right);

// Direct support through the runtime for now
void print_string__1(String text);
String read_string__0();
String _uint__to_display_string__0(_uint value);
