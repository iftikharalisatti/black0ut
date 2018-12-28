// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

namespace Black0ut.Runtime
{
    public enum InstructionType : byte
    {
        // PROGRAM
        NOOPERATION,
        RETURN,
        EXECUTE,
        READ,
        IF,
        COPY,

        // NOT LINK INSTRUCTION
        _GOTO,
        _SKIP,
        _LOG,

        // ARITHMETIC
        INCREMENT,
        DECREMENT,
        ADD,
        SUBTRACT,
        MULTIPLY,
        DIVIDE,
        REMAINDER,
        SHIFTLEFT,
        SHIFTRIGHT,
        XOR,
        SQRT,
        
        // COMPILER
        LINK,
    }
}
