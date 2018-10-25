
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
            try
            {
                var m = Module.Manager["Vx"];
                m.Execute(new[] { "" }, out var _);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadKey();
            return;
        }
    }
}
