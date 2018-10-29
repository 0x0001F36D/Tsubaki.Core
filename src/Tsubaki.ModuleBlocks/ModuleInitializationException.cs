

namespace Tsubaki.ModuleBlocks
{
    using System;

    internal sealed class ModuleInitializationException : Exception
    {
        internal ModuleInitializationException(Type type, string reason):base(reason)
        {
            this.Type = type;
            this.Reason = reason;
        }

        public Type Type { get; }
        public string Reason { get; }
    }
    
}
