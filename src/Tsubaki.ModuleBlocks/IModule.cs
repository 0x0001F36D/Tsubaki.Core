

namespace Tsubaki.ModuleBlocks
{
    using System.ComponentModel.Composition;
    using Tsubaki.ModuleBlocks.Enums;

    [InheritedExport]
    public interface IModule
    {
        string Name { get; }

        ModuleScopes Scopes { get; }

        void OnInitialize();

        bool Execute(string[] args, out object callback);
        
    }

    
}
