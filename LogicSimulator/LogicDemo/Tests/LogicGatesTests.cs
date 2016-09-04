#region References

using Logic.Model;
using Logic.Model.Core;
using Logic.Model.Gates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Diagnostics;

#endregion

namespace Logic.Tests
{
    #region Tests: Digital Logic Gates

    public static class LogicGatesTests
    {
        #region AndGateTests

        public static void AndGateTests()
        {
            Debug.Print("Running AndGate Tests:");

            // 01
            {
                var g = new AndGate();
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 02
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 03
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 04
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 05
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 06
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 07
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            // 08
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            //09
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 10
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 11
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 12
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 13
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 14
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 15
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            // 16
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 17
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 18
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 19
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 20
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            // 21
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 22
            {
                var g = new AndGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            // 23
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
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            // 24
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
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            Debug.Print("Done AndGate Tests\n");
        }

        #endregion

        #region OrGateTests

        public static void OrGateTests()
        {
            Debug.Print("Running OrGate Tests:");

            {
                var g = new OrGate();
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new OrGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            Debug.Print("Done OrGate Tests\n");
        }

        #endregion

        #region NotGateTests

        public static void NotGateTests()
        {
            Debug.Print("Running NotGate Tests:");

            {
                var g = new NotGate();
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new NotGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new NotGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            Debug.Print("Done NotGate Tests\n");
        }

        #endregion

        #region BufferGateTests

        public static void BufferGateTests()
        {
            Debug.Print("Running BufferGate Tests:");

            {
                var g = new BufferGate();
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == null);
                Debug.Print("{1} => [null] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == null ? "ok" : "nok");
            }

            {
                var g = new BufferGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new BufferGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            Debug.Print("Done BufferGate Tests\n");
        }

        #endregion

        #region NandGateTests

        public static void NandGateTests()
        {
            Debug.Print("Running NandGate Tests:");

            {
                var g = new NandGate();
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new NandGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new NandGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new NandGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new NandGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new NandGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new NandGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            Debug.Print("Done NandGate Tests\n");
        }

        #endregion

        #region NorGateTests

        public static void NorGateTests()
        {
            Debug.Print("Running NorGate Tests:");

            {
                var g = new NorGate();
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new NorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new NorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new NorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new NorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new NorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new NorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            Debug.Print("Done NorGate Tests\n");
        }

        #endregion

        #region XorGateTests

        public static void XorGateTests()
        {
            Debug.Print("Running XorGate Tests:");

            {
                var g = new XorGate();
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            Debug.Print("Done XorGate Tests\n");
        }

        #endregion

        #region XnorGateTests

        public static void XnorGateTests()
        {
            Debug.Print("Running XnorGate Tests:");

            {
                var g = new XnorGate();
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");

            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(false));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == false);
                Debug.Print("{1} => [false] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == false ? "ok" : "nok");
            }

            {
                var g = new XnorGate();
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Inputs.Add(new DigitalSignal(true));
                g.Outputs.Add(new DigitalSignal());
                g.Calculate();
                Debug.Assert(g.Outputs.First().State == true);
                Debug.Print("{1} => [true] g={0}", g.Outputs.First().State.ToString(), g.Outputs.First().State == true ? "ok" : "nok");
            }

            Debug.Print("Done XnorGate Tests\n");
        }

        #endregion
    }

    #endregion
}
