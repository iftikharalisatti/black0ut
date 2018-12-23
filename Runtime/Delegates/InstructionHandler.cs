// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

namespace Black0ut.Runtime
{
    /// <summary>
    /// Return true on program return
    /// </summary>
    public delegate bool InstructionHandler(byte[] file, ref byte executionPoint);
}
