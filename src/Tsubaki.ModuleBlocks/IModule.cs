

namespace Tsubaki.ModuleBlocks
{
    using System.ComponentModel.Composition;
    using Tsubaki.ModuleBlocks.Enums;

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
    
}
