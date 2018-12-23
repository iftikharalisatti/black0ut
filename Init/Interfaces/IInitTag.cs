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
    public interface IInitTag
    {
        /// <summary>
        /// Init class tag, when need init more than one group
        /// </summary>
        string Tag { get; set; }

        /// <summary>
        /// Initialize class
        /// </summary>
        void Init();

        /// <summary>
        /// Initialize class after other classes Init
        /// </summary>
        void PostInit();
    }
}