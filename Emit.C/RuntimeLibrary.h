#include <stdint.h>

// Rather than including all of stdlib.h we only declare the one thing we need.
void* malloc(size_t size);

#define _UNUSED(x) (void)(x)

// Integer types
typedef struct { int32_t _value; } _int;
typedef struct { uint32_t _value; } _uint;
typedef struct { uint8_t _value; } _byte;
typedef struct { uintptr_t _value; } _size;
typedef struct { intptr_t _value; } _offset;

inline _int _int__add(_int x, _int y) { return (_int){ x._value + y._value }; }

// String type
// Note: For now, we have moved strings
typedef struct {
    _size byte_count;
    _byte* bytes;
} String;

inline String String___op_string_literal__2(_size byte_count, _byte* bytes)
{
    return (String){ byte_count, bytes };
}

// Direct support for console IO through the runtime for now
void print_string__1(String text);
