#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Logic.Model;
using Logic.Model.Gates;
using Logic.Model.Core; 

#endregion

namespace Tests
{
    #region UnitTestAndGate

    [TestClass]
    public class UnitTestAndGate
    {
        [TestMethod]
        public void AndGateTest00()
        {
            var g = new AndGate();

            Assert.IsNotNull(g);

            Assert.IsNotNull(g.Inputs);
            Assert.IsNotNull(g.Outputs);
            Assert.IsNotNull(g.Pins);

            Assert.IsTrue(g.Inputs.Count == 0);
            Assert.IsTrue(g.Outputs.Count == 0);
            Assert.IsTrue(g.Pins.Count == 0);
        }

        [TestMethod]
        public void AndGateTest01()
        {
            var g = new AndGate();
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest02()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest03()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest04()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest05()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(false));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest06()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest07()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(true, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest08()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest09()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest10()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(false));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest11()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest12()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest13()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest14()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(false));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest15()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(true, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest16()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest17()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest18()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest19()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest20()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(true, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest21()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest22()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(true, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest23()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Inputs.Add(new DigitalSignal(false));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(false, g.Outputs.First().State);
        }

        [TestMethod]
        public void AndGateTest24()
        {
            var g = new AndGate();
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Inputs.Add(new DigitalSignal(true));
            g.Outputs.Add(new DigitalSignal());
            g.Calculate();

            Assert.AreEqual(true, g.Outputs.First().State);
        }
    } 
    
    #endregion
}
