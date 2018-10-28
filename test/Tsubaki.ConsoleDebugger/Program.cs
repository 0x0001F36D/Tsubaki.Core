
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
    using Tsubaki.Core;

    class Program
    { 
        static void Main(string[] args)
        {
            /*
            var module = Module.Manager["V"];
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
