﻿#include "RuntimeLibrary.h"
#include <stdio.h>
#include <string.h>

// Reminder: `extern` function declarations are so the compiler knows what
// object file to put the non-inline copies of inlined functions in.

#define EXTERN_OPTIONAL_TYPE(type) \
extern inline _opt__##type _opt__##type##__Some(type x); \
const _opt__##type _opt__##type##__none = (_opt__##type) {0};

// `bool` type
extern inline _bool _bool__and(_bool x, _bool y);
extern inline _bool _bool__or(_bool x, _bool y);

EXTERN_OPTIONAL_TYPE(_bool)

// Extern Integer Type Operations Macro
#define EXTERN_INTEGER_OPERATIONS(type) \
extern inline type type##__neg(type x); \
extern inline type type##__add(type x, type y); \
extern inline type type##__sub(type x, type y); \
extern inline type type##__mul(type x, type y); \
extern inline type type##__div(type x, type y); \
extern inline type type##__remainder__2(type x, type y); \
extern inline _bool type##__eq(type x, type y); \
extern inline _bool type##__ne(type x, type y); \
extern inline _bool type##__lt(type x, type y); \
extern inline _bool type##__lte(type x, type y); \
extern inline _bool type##__gt(type x, type y); \
extern inline _bool type##__gte(type x, type y);

// `int` type
EXTERN_INTEGER_OPERATIONS(_int)
EXTERN_OPTIONAL_TYPE(_int)

// `uint` type
EXTERN_INTEGER_OPERATIONS(_uint)

// `byte` type
EXTERN_INTEGER_OPERATIONS(_byte)

// `size` type
EXTERN_INTEGER_OPERATIONS(_size)

// `offset` type
EXTERN_INTEGER_OPERATIONS(_offset)

// conversion functions
extern inline _int _convert___byte___int(_byte value);

// Forward declare String that is in std lib for use by intrinsics
typedef struct String___Self String___Self;
typedef struct String___VTable String___VTable;
struct String
{
    String___VTable const* restrict _vtable;
    String___Self* restrict _self;
};
extern String___VTable String___vtable;

String String___new__3(String _self, _size byte_count, _size bytes);

// Intrinsic Functions
String _uint__to_display_string__1(_uint value)
{
    int length = snprintf(NULL, 0, "%u", value._value);
    char* str = malloc(length + 1);
    snprintf(str, length + 1, "%u", value._value);
    String instance = (String){&String___vtable, malloc(sizeof(String))};
    _size byte_count = (_size) { length };
    _size bytes = (_size) { (uintptr_t)str };
    return String___new__3(instance, byte_count, bytes);
}
_size intrinsics__mem_allocate__1(_size length)
{
    void* mem = malloc(length._value);
    return (_size) { (uintptr_t)mem };
}
void intrinsics__mem_deallocate__1(_size ptr)
{
    free((void*)ptr._value);
}
void intrinsics__mem_copy__3(_size from_ptr, _size to_ptr, _size length)
{
    memcpy((void*)to_ptr._value, (void*)from_ptr._value, length._value);
}
extern inline void intrinsics__mem_set_byte__2(_size ptr, _byte byte);
extern inline _byte intrinsics__mem_get_byte__1(_size ptr);
void intrinsics__print_utf8__2(_size ptr, _size length)
{
    printf("%.*s", (int)length._value, (char*)ptr._value);
}
_size intrinsics__read_utf8_line__2(_size ptr, _size length)
{
    gets_s((char*)ptr._value, length._value);
    return (_size) { strnlen_s((char*)ptr._value, 1024) };
}

// Test of calling windows memory allocation functions, rather than including
// windows.h, we directly declare the functions. This demonstrates that external
// function calls could do this.
//#include <windows.h>
typedef void* LPVOID;
typedef LPVOID HANDLE;
typedef size_t SIZE_T;
typedef uint32_t DWORD;
typedef int32_t BOOL;

HANDLE GetProcessHeap();
LPVOID HeapAlloc(HANDLE hHeap, DWORD dwFlags, SIZE_T dwBytes);
LPVOID HeapReAlloc(HANDLE hHeap, DWORD dwFlags, LPVOID lpMem, SIZE_T dwBytes);
BOOL HeapFree(HANDLE hHeap, DWORD dwFlags, LPVOID lpMem);
DWORD GetLastError();

void test()
{
    void* ptr = HeapAlloc(GetProcessHeap(), 0, 10);
    HeapFree(GetProcessHeap(), 0, ptr);
    //void* ptr = aligned_alloc(8, 32);
    //free(ptr);
}
