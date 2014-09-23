using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Marv.Test
{
    [TestClass]
    public class TestSmile
    {
        [TestMethod]
        public void TestGetData()
        {
            var network = new Smile.Network();
            network.ReadFile("Resources/FreeCorp.net");
        }
    }
}
