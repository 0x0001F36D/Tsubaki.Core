using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsubaki.ModuleBlocks
{
    [InheritedExport]
    public interface IModule
    {
        string Name { get; }

        InitializationMode InitializationMode { get;  }

        void Initialize();

        NextStep Execute(string[] args);

        IModuleSetting Setting { get; }
    }

    public interface IModuleSetting
    {
        void Write(string key, string value);
        (string key, string value) Read();
    }

    public abstract class ModuleBase : IModule
    {
        public virtual string Name { get; private set; }

        public virtual InitializationMode InitializationMode { get; }

        public IModuleSetting Setting { get; }

        protected ModuleBase()
        {
            if (InitializationMode == InitializationMode.OnCreate)
                this.Initialize();
        }

        public virtual void Initialize()
        {
        }

        protected abstract NextStep ExecuteImpl(string[] args);

        public NextStep Execute(string[] args)
        {
            if (this.InitializationMode == InitializationMode.BeforeExecute)
                this.Initialize();
            return this.ExecuteImpl(args ?? Array.Empty<string>());
        }

    }
    
    public enum InitializationMode
    {
        OnCreate,
        BeforeExecute,
    }

    // move next
    // break
    public sealed class NextStep
    {
        private readonly Lazy<ModuleBase> _lazy;
        private readonly string[] _args;

        public static NextStep Break = new NextStep(false);

        public static NextStep Next<TModule>(string[] args) 
            where TModule : ModuleBase, new()
        {
            var next = new NextStep(new Lazy<ModuleBase>(() => new TModule()), args);

            return next;
        }

        private NextStep(Lazy<ModuleBase> lazy,string[] args) : this(true)
        {
            this._lazy = lazy;
            this._args = args;
        }

        private NextStep(bool hasNext)
        {
            this.HasNext = hasNext ;
        }

        public NextStep GetNext()
        {
            if (this)
                return this._lazy.Value.Execute(this._args);

            return NextStep.Break;
        }

        public bool HasNext { get; }


        public static implicit operator bool(NextStep ns)
        {
            return ns.HasNext;
        }

    }

    class TestMD : ModuleBase
    {
        protected override NextStep ExecuteImpl(string[] args)
        {
            foreach (var arg in args)
            {

                Console.WriteLine(arg);
            }

            return NextStep.Next<TestMD>(args);
        }
    }
    class MainClass
    {
        static void Main(string[] args)
        {
            var mod = new TestMD();

            var ex = mod.Execute(new[] { "A","B" });
            while (ex.GetNext()) ;


            Console.ReadKey();
            return;
        }
    }
}
