namespace Tsubaki.ModuleBlocks
{
    using System;
    using System.ComponentModel.Composition;

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ModuleAttribute : ExportAttribute, IModuleMetadata
    {
        public ModuleAttribute(string moduleName, params string[] keywords) : base(typeof(IModule))
        {
            this.Name = moduleName;
            this.Keywords = keywords;
        }

        public string Name { get; }
        public string[] Keywords { get; }
    }
    
}
