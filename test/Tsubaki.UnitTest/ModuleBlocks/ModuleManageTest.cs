
namespace Tsubaki.UnitTest.ModuleBlocks
{
    using System;
    using NUnit.Framework;
    using Tsubaki.ModuleBlocks;

    [TestFixture]
    public class ModuleManageTest
    {

        [TestCase]
        public void HasMetadata()
        {
            var m = Tsubaki.ModuleBlocks.Module.Manager["Has"];
            var result = m.Execute(Array.Empty<string>(), out var callback);

            Assert.True(result);
            Assert.AreEqual(callback, "Success");
        }


        [TestCase]
        public void NoMetadata()
        {
            Assert.Catch(() =>
            {
                var m = Module.Manager["NoMetadata"];
            });

        }


    }



    public sealed class NoMetadata : ModuleBase
    {

        protected override bool ExecuteImpl(string[] args, ref object callback)
        {
            callback = "Success";
            return true;
        }

    }


    [Module("Has", "s","v")]
    public sealed class HasMetadata : ModuleBase
    {

        protected override bool ExecuteImpl(string[] args, ref object callback)
        {
            callback = "Success";
            return true;
        }

    }
}
