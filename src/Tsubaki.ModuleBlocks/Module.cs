

namespace Tsubaki.ModuleBlocks
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using ModuleBlocks.Metadata;
    using Conditions.Guards;
    using Tsubaki.ModuleBlocks.Internal;

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
        private Lazy<IModule, IModuleMetadata>[] _modules;
#pragma warning restore 0649

        private CompositionContainer _container;

        private Module()
        {
            var catalog = new AggregateCatalog();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Debug.WriteLine("Composited assembly: " + assembly.FullName);
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
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
            catch (CompositionException compositionException)
            {
                Debug.WriteLine("Composite Error: " +compositionException.ToString());
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
            Check.If(name).IsNotNullOrEmpty();

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
    }
    
}
