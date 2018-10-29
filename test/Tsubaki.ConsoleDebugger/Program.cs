
namespace Tsubaki.ConsoleDebugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Tsubaki.ModuleBlocks;

    using ApiAi;
    using Newtonsoft.Json;
    using Tsubaki.ModuleBlocks.Managment;

    [Module("Fake", "Test", "Fake")]
    class MFake : ModuleBase
    {
        protected override bool ExecuteImpl(string[] args, ref object callback)
        {
            if (args.Contains("fake"))
                return true;

            else
                return false;
        }
    }

    [Module("Real", "NotATest", "Real")]
    class MReal : ModuleBase
    {
        protected override bool ExecuteImpl(string[] args, ref object callback)
        {
            if (args.Contains("real"))
            {
                callback = "Success";
                return true;
            }
            else
            {
                callback = "failure";
                return false;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {


            var result = Module.Manager.Execute(
                keywords: new[] { "Real" },
                args: new[] { "real" },
                callback: out var callback);

            Console.WriteLine(result);
            Console.WriteLine(callback);
            Console.WriteLine();
            
            /*
            var module = Module.Manager["v"];
            if (module.Execute(new[] 
            {
                "--target",
                "https://fb.me",
                "--exe",
                "chrome.exe",
            }, out var c))
            {
                Console.WriteLine(c);
            }
            */
            Console.ReadKey();
            return ;
        }

        static void Apiai()
        {

            var req = QueryService.SendRequest(new ApiAi.Models.ConfigModel
            {
                AccesTokenClient = ""
            }, "");
            var result = req.GetType().GetProperties().Select(x => new
            {
                //Type = x.PropertyType.FullName,
                x.Name,
                Value = x.GetValue(req)
            }).ToList();
            Console.WriteLine(result.DebugString());

          

        }
    }

    static class Ex
    {
        public static string DebugString(this object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented);
        }

    }
}
