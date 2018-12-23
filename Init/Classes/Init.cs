// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;
using System.Linq;
using System.Collections.Generic;

namespace Black0ut.Init
{
    public static class Init
    {
        /// <summary>
        /// Initialize all objects with interface IInit
        /// </summary>
        public static void InitAll()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IInit).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            var inits = new List<IInit>();

            foreach (var subType in types)
            {
                inits.Add((IInit)Activator.CreateInstance(subType));
            }

            // Init
            foreach (var init in inits)
            {
                init.Init();
            }

            // PostInit
            foreach (var init in inits)
            {
                init.PostInit();
            }
        }

        /// <summary>
        /// Initialize all objects with interface IInitTag and specifed tag
        /// </summary>
        public static void InitAll(string tag)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IInitTag).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            var inits = new List<IInitTag>();

            foreach (var subType in types)
            {
                inits.Add((IInitTag)Activator.CreateInstance(subType));
            }

            // Init
            foreach (var init in inits)
            {
                if(tag.Equals(init.Tag))
                    init.Init();
            }

            // PostInit
            foreach (var init in inits)
            {
                if (tag.Equals(init.Tag))
                    init.PostInit();
            }
        }
    }
}