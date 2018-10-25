
namespace Tsubaki.ConsoleDebugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Tsubaki.ModuleBlocks;

    class Program
    {
        static void Main(string[] args)
        {
            var m = LoadLyb.Instance["Vx"];
            m.Execute(new[] { "" }, out var _);

            Console.ReadKey();
            return;
        }
    }
}
