

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

        protected ModuleBase()
        {
            this.OnInitialize();
        }

        public virtual void OnInitialize()
        {
        }

        protected abstract bool ExecuteImpl(string[] args, ref object callback);


        public bool Execute(string[] args, out object callback)
        {
            callback = null;
            return this.ExecuteImpl(args ,ref callback);
        }

    }
    
}
