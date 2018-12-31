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
        #region Without Tag

        /// <summary>
        /// Initialize all objects with interface IInit
        /// </summary>
        public static void InitAll()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IInit).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            var initObjects = new List<IInit>();

            foreach (var subType in types)
            {
                initObjects.Add((IInit)Activator.CreateInstance(subType));
            }

            // Init
            foreach (var init in initObjects)
            {
                init.Init();
            }

            // PostInit
            foreach (var init in initObjects)
            {
                init.PostInit();
            }
        }

        /// <summary>
        /// Call Init() on all objects with interface IInit
        /// </summary>
        public static void CallInit()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IInit).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            var initObjects = new List<IInit>();

            foreach (var subType in types)
            {
                initObjects.Add((IInit)Activator.CreateInstance(subType));
            }

            // Init
            foreach (var init in initObjects)
            {
                init.Init();
            }
        }

        /// <summary>
        /// Call PostInit() on all objects with interface IInit
        /// </summary>
        public static void CallPostInit()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IInit).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            var initObjects = new List<IInit>();

            foreach (var subType in types)
            {
                initObjects.Add((IInit)Activator.CreateInstance(subType));
            }

            // PostInit
            foreach (var init in initObjects)
            {
                init.PostInit();
            }
        }

        #endregion
        #region With Tag

        /// <summary>
        /// Initialize all objects with interface IInitTag and specifed tag
        /// </summary>
        public static void InitAll(string tag)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IInitTag).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            var initObjects = new List<IInitTag>();

            foreach (var subType in types)
            {
                initObjects.Add((IInitTag)Activator.CreateInstance(subType));
            }

            // Init
            foreach (var init in initObjects)
            {
                if (tag.Equals(init.Tag))
                    init.Init();
            }

            // PostInit
            foreach (var init in initObjects)
            {
                if (tag.Equals(init.Tag))
                    init.PostInit();
            }
        }

        /// <summary>
        /// Call Init() on all objects with interface IInitTag and specifed tag
        /// </summary>
        public static void CallInit(string tag)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IInitTag).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            var initObjects = new List<IInitTag>();

            foreach (var subType in types)
            {
                initObjects.Add((IInitTag)Activator.CreateInstance(subType));
            }

            // Init
            foreach (var init in initObjects)
            {
                if (tag.Equals(init.Tag))
                    init.Init();
            }
        }

        /// <summary>
        /// Call PostInit() on all objects with interface IInitTag and specifed tag
        /// </summary>
        public static void CallPostInit(string tag)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IInitTag).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            var initObjects = new List<IInitTag>();

            foreach (var subType in types)
            {
                initObjects.Add((IInitTag)Activator.CreateInstance(subType));
            }

            // PostInit
            foreach (var init in initObjects)
            {
                if (tag.Equals(init.Tag))
                    init.PostInit();
            }
        }

        #endregion
    }
}