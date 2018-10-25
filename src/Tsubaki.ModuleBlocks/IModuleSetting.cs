

namespace Tsubaki.ModuleBlocks
{
    public interface IModuleSetting
    {
        void Write(string key, string value);
        (string key, string value)[] Read();
        string this[string key] { get; set; }

    }
    
}
