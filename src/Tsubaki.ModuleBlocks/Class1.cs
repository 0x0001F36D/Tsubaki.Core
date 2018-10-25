
namespace Tsubaki.ModuleBlocks
{using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
using System.Threading.Tasks;

    
    
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

    [Flags]
    public enum ModuleScopes
    {
        None = 0
    }


    // [InheritedExport]
    public abstract class ModuleBase  : IModule
    {
        public abstract ModuleScopes Scopes { get; }
        public abstract string Name { get;  }

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

    public interface IModuleMetadata
    {
        string Name { get; }
    }


    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ModuleAttribute : ExportAttribute, IModuleMetadata
    {
        public ModuleAttribute(string name)
           : base(typeof(IModule))
        {
            this._name = name;
        }
        private string _name;

        string IModuleMetadata.Name => this._name;
    }
    


    public enum InitializationMode
    {
        OnCreate,
        Everytime,
    }

    [Module("TestMD")]
     public class TestMD : ModuleBase
    {
        public override InitializationMode InitializationMode => InitializationMode.OnCreate;
        public override string Name => "Test";

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

        [ImportMany]
        private IEnumerable< Lazy<IModule, IModuleMetadata>> _modules;

        private LoadLyb()
        {
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

            //Create the CompositionContainer with the parts in the catalog
            this._container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        public IModule this[string name, bool ignoreCase = false]
        {
            get
            {
                Console.WriteLine(this._modules.Count());

                foreach (var m in this._modules)
                {
                    System.Diagnostics.Debug.WriteLine("MDK: "+m.Metadata.Name);
                    if (string.Equals(name, m.Metadata.Name, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
                    {
                        return m.Value;
                    }
                }
                throw new KeyNotFoundException(name);
            }
        }
        private CompositionContainer _container;


        public void D()
        {
            foreach (var item in this._modules)
            {
                Console.WriteLine(item.Metadata.Name);
            }
        }
    }


    class MainClass
    {
        static void Main(string[] args)
        {

            LoadLyb.Instance.D();
            //m.Execute(null, out var _);


            Console.ReadKey();
            return;
        }
    }
}
