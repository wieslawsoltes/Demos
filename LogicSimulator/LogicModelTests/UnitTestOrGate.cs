#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Logic.Model;
using Logic.Model.Gates; 

#endregion

namespace Tests
{
    #region UnitTestOrGate

    [TestClass]
    public class UnitTestOrGate
    {
        [TestMethod]
        public void OrGateTest00()
        {
            var g = new OrGate();

            Assert.IsNotNull(g);

            Assert.IsNotNull(g.Inputs);
            Assert.IsNotNull(g.Outputs);
            Assert.IsNotNull(g.Pins);

            Assert.IsTrue(g.Inputs.Count == 0);
            Assert.IsTrue(g.Outputs.Count == 0);
            Assert.IsTrue(g.Pins.Count == 0);
        }
    } 

    #endregion
}
