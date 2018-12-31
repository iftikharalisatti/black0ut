// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

namespace Black0ut.Init
{
    public interface IInit
    {
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