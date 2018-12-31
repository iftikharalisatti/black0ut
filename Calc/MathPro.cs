// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

namespace Black0ut.Calc
{
    public static class MathPro
    {
        /// <summary>
        /// Returns distance between 2 dots in 3d space, without sqrt().
        /// </summary>
        /// <returns></returns>
        public static float DistanceInPow3D(float[] a, float[] b)
        {
            return (b[0] - a[0]) * (b[0] - a[0]) + (b[1] - a[1]) * (b[1] - a[1]) + (b[2] - a[2]) * (b[2] - a[2]);
        }
    }
}
