

namespace Tsubaki.ModuleBlocks.Managment
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;

    using Tsubaki.ModuleBlocks.Internal;
    using System.IO;
    using System.Reflection;
    using System.Collections.Generic;

    public sealed class Module
    {
        public static Module Manager
        {
            get
            {
                lock (s_locker)
                {
                    if (s_instance == null)
                    {
                        lock (s_locker)
                        {
                            s_instance = new Module();
                        }
                    }
                    return s_instance;
                }
            }
        }

        private readonly static object s_locker = new object();
        private static volatile Module s_instance;

#pragma warning disable 0649
        [ImportMany]
        private Lazy<IModule, IModuleMetadata>[] _modules = new Lazy<IModule, IModuleMetadata>[0];
#pragma warning restore 0649

        private CompositionContainer _container;

        private Module()
        {
            var catalog = new AggregateCatalog();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Debug.WriteLine("Prepare assembly for composition: " + assembly.FullName);
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }

            var dir = new DirectoryInfo("./Modules/");
            if (!dir.Exists)
            {
                dir.Create();
            }
            foreach (var sub in dir.GetDirectories())
            {
                var f = new DirectoryCatalog(sub.FullName, "*.dll");
                catalog.Catalogs.Add(f);
            }
            


            this._container = new CompositionContainer(catalog);

            try
            {
                this._container.ComposeParts(this);
                foreach (var m in this._modules)
                {
                    Debug.WriteLine("Loaded module: " + m.Metadata.Name);
                }
            }
            catch(TypeLoadException e)
            {
                //版本不同
                Debug.WriteLine("Loaded Error: " + e.ToString());
            }
            catch (ReflectionTypeLoadException e)
            {
                Debug.WriteLine("Loaded Error: " + e.ToString());
                foreach (var ex in e.LoaderExceptions)
                {
                    Debug.WriteLine(ex.Message); 
                }
            }
            catch (CompositionException e)
            {
                Debug.WriteLine("Composite Error: " +e.ToString());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public IModule this[string name, bool ignoreCase = false] 
            => this.Get(name, ignoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ModuleNotFoundException"/>
        public IModule Get(string name, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("message", nameof(name));

            var ic = ignoreCase 
                ? StringComparison.CurrentCultureIgnoreCase 
                : StringComparison.CurrentCulture;

            foreach (var lazy in this._modules)
            {
                if (string.Equals(lazy.Metadata.Name, name, ic))
                    return lazy.Value;
            }

            throw new ModuleNotFoundException(name);
        }

        public void All()
        {
            foreach (var item in this._modules)
            {
                Debug.WriteLine(item.Metadata.Name);
            }
            Debug.WriteLine("-----");
        }

        private static class Diff
        {
            private static HashSet<string> GetSet(IEnumerable<string> strings)
            {
                var hs = new HashSet<string>();
                foreach (var str in strings)
                {
                    hs.Add(str.ToLower());
                }
                return hs;
            }


            public static double Compare(IEnumerable<string> src, IEnumerable<string> cmp)
            {
                if (src is null || cmp is null)
                    return 0.0;

                var s = GetSet(src);
                var c = GetSet(cmp);

                var t = s.Count + c.Count;

                s.IntersectWith(c);
                return (double)s.Count / t;

            }
        }

        public bool Execute(string[] keywords, string[] args, out object callback)
        {
            var top_v = 0.0;
            var top = default(Lazy<IModule, IModuleMetadata>);

            foreach (var m in this._modules)
            {
                var diff = Diff.Compare(m.Metadata.Keywords, keywords);
                if (diff > top_v)
                {
                    top_v = diff;
                    top = m;
                }
            }

            //Found the highest similar object
            return top.Value.Execute(args, out callback);

        }
    }
    
}
