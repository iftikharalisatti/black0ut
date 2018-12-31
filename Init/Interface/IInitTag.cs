// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

namespace Black0ut.Init
{
    /// <summary>
    /// Init interface with tag
    /// </summary>
    public interface IInitTag : IInit
    {
        /// <summary>
        /// Init class tag, when need init more than one group
        /// </summary>
        string Tag { get; set; }
    }
}