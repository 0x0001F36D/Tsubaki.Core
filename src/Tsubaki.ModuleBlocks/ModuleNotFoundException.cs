

namespace Tsubaki.ModuleBlocks
{
    using System;
    using Tsubaki.ModuleBlocks.Internal;

    public static class Exceptions
    {
        public static void ModuleNotFound(string moduleName)
        {
            throw new ModuleNotFoundException(moduleName);
        }
    }
}

namespace Tsubaki.ModuleBlocks.Internal
{
    using System;

    internal static class Strings
    {
        internal static string ModuleNotFound = "Module not found: {0}";
        

        internal static string Build(this string str,  params object[] arg)
        {
            return string.Format(str, arg);
        }
    }

    public sealed class ModuleNotFoundException : Exception
    {
        internal ModuleNotFoundException(string moduleName) : base(Strings.ModuleNotFound.Build( moduleName))
        {
            this.ModuleName = moduleName ?? throw new ArgumentNullException(nameof(moduleName));
        }

        public string ModuleName { get; }
    }

}
