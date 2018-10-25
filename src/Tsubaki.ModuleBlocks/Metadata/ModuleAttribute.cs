namespace Tsubaki.ModuleBlocks.Metadata
{
    using System;
    using System.ComponentModel.Composition;

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ModuleAttribute : ExportAttribute, IModuleMetadata
    {
        public ModuleAttribute(string moduleName) : base(typeof(IModule))
        {
            this.Name = moduleName;
        }

        public string Name { get; }
    }
}
