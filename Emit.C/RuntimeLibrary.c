#include "RuntimeLibrary.h"
#include <stdio.h>

// Reminder: `extern` function declarations are so the compiler knows what
// object file to put the non-inline copies of inlined functions in.

// Integer types
extern inline _int _int__add(_int x, _int y);

// String type
extern String String___op_string_literal__2(_size count, _byte* bytes);

// Direct support for console IO through the runtime for now
void print_string__1(String text)
{
    printf("%.*s\n", (int)text.byte_count._value, (char*)text.bytes);
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
