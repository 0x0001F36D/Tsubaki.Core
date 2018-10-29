

namespace Tsubaki.ModuleBlocks
{
    using System.ComponentModel.Composition;

    /// <summary>
    /// 提供對模組的基本功能
    /// </summary>
    [InheritedExport]
    public interface IModule
    {
        /// <summary>
        /// 名稱
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 關鍵字
        /// </summary>
        string[] Keywords { get; }
        
        /// <summary>
        /// 執行
        /// </summary>
        /// <param name="args"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        bool Execute(string[] args, out object callback);
        
    }

    
}
