

namespace Tsubaki.ModuleBlocks
{
    using System.Reflection;
    using ModuleBlocks.Metadata;
    using Tsubaki.ModuleBlocks.Enums;

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
    
}
