

namespace Tsubaki.ModuleBlocks
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using ModuleBlocks.Metadata;
    using Tsubaki.ModuleBlocks.Enums;

    using System.Diagnostics.CodeAnalysis;
    using Conditions.Guards;

    [InheritedExport]
    public interface IModule
    {
        string Name { get; }

        ModuleScopes Scopes { get; }

        InitializationMode InitializationMode { get;  }

        void Initialize();

        bool Execute(string[] args, out object callback);

        IModuleSetting Setting { get; }
    }

    public interface IModuleSetting
    {
        void Write(string key, string value);
        (string key, string value)[] Read();
        string this[string key] { get; set; }

    }

    public abstract class ModuleBase  : IModule
    {
        public virtual string Name
        {
            get
            {
                var t = this.GetType();
                if (t.GetCustomAttribute<ModuleAttribute>() is ModuleAttribute m)
                {
                    return m.Name;
                }
                return t.Name;
            }
        }

        public abstract ModuleScopes Scopes { get; }
        
        public virtual InitializationMode InitializationMode { get; }

        public IModuleSetting Setting { get; }

        protected ModuleBase()
        {
            if (InitializationMode == InitializationMode.OnCreate)
                this.Initialize();
        }

        public virtual void Initialize()
        {
        }

        protected abstract bool ExecuteImpl(string[] args, out object callback);


        public bool Execute(string[] args, out object callback)
        {
            if (this.InitializationMode == InitializationMode.Everytime)
                this.Initialize();
            return this.ExecuteImpl(args ,out callback);
        }

    }

    [Module("V")]
    public class TestMD : ModuleBase
    {
        public override InitializationMode InitializationMode => InitializationMode.OnCreate;

        public override ModuleScopes Scopes => ModuleScopes.None;
        protected override bool ExecuteImpl(string[] args, out object callback)
        {
            callback = false;
            Console.WriteLine("FAKE INVOKE");
            return true;
        }
    }



    public sealed class LoadLyb
    {
        public static LoadLyb Instance
        {
            get
            {
                lock (s_locker)
                {
                    if (s_instance == null)
                    {
                        lock (s_locker)
                        {
                            s_instance = new LoadLyb();
                        }
                    }
                    return s_instance;
                }
            }
        }

        private readonly static object s_locker = new object();
        private static volatile LoadLyb s_instance;

#pragma warning disable 0649
        [ImportMany]
        private Lazy<IModule, IModuleMetadata>[] _modules;
#pragma warning restore 0649
        private CompositionContainer _container;


        private LoadLyb()
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

        public IModule this[string name, bool ignoreCase = false] 
            => this.Get(name, ignoreCase);

        public IModule Get(string name, bool ignoreCase = false)
        {
            Check.If(name).IsNotNullOrEmpty();

            var ic = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            foreach (var lazy in this._modules)
            {
                var n = lazy.Metadata is IModuleMetadata m ? (m.Name ?? lazy.Value.Name) : lazy.Value.Name;
                if (string.Equals( n ,name, ic))
                    return lazy.Value;
            }



            throw new ModuleNotFoundException(name);
        }
    }

    public sealed class ModuleNotFoundException : Exception
    {
        internal ModuleNotFoundException(string moduleName)
        {
            this.ModuleName = moduleName ?? throw new ArgumentNullException(nameof(moduleName));
        }

        public string ModuleName { get; }
    }
    
}
