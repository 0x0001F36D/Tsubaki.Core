

namespace Tsubaki.ModuleBlocks
{
    using System.Diagnostics;
    using System.Reflection;

    public abstract class ModuleBase  : IModule
    {
        public string Name { get; }
        public string[] Keywords { get; }

        protected ModuleBase()
        {
            var t = this.GetType();
            if (t.GetCustomAttribute<ModuleAttribute>() is IModuleMetadata metadata)
            {
                this.Name = metadata.Name ?? t.Name;
                if (metadata.Keywords is string[] keys && keys.Length > 0)
                {
                    this.Keywords = metadata.Keywords;
                }
                else 
                {
                    throw new ModuleInitializationException(t, "The Keywords is empty so this module will not be invoked");
                }
            }
            else
            {
                throw new ModuleInitializationException(t, $"The { nameof(ModuleAttribute)} not depend on type {t.Name}, Add the attribute can improve this exception");
            }
            
            this.OnInitialize();
        }

        /// <summary>
        /// 在初始化時引發
        /// </summary>
        protected virtual void OnInitialize()
        {
        }


        /// <summary>
        /// 執行內容實做
        /// </summary>
        /// <param name="args"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected abstract bool ExecuteImpl(string[] args, ref object callback);

        bool IModule.Execute(string[] args, out object callback)
        {
            callback = null;
            return this.ExecuteImpl(args ,ref callback);
        }

    }
    
}
