
namespace Logic.TruthTables
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region Program

    class Program
    {
        static void Main(string[] args)
        {
            // http://en.wikipedia.org/wiki/Three-valued_logic
            // Set of truth tables showing the logic operations for Kleene's logic

            // If numeric values are assigned to FALSE, NULL and TRUE 
            // such that FALSE is less than NULL and NULL is less than TRUE, 
            // then A AND B AND C... = MIN(A, B, C ...) and A OR B OR C ... = MAX(A, B, C...).
            // example: FALSE = 0, NULL = 1, TRUE = 2

            // A AND B  | True     | Unknown  | False
            // ---------|----------|----------|--------
            // True     | True     | Unknown  | False
            // Unknown  | Unknown  | Unknown  | False
            // False    | False    | False    | False

            // A OR B   | True     | Unknown  | False
            // ---------|----------|----------|--------
            // True     | True     | True     | True
            // Unknown  | True     | Unknown  | Unknown
            // False    | True     | Unknown  | False

            // A        | NOT A
            // ---------|----------
            // True     | False
            // Unknown  | Unknown
            // False    | True

            bool?[] states = new bool?[3] { true, null, false };

            Console.WriteLine("A AND B");
            foreach (bool? A in states)
            {
                foreach (bool? B in states)
                {
                    var R = A & B;
                    Console.WriteLine("{0}\t&\t{1}\t= {2}",
                        A == null ? "null" : A.ToString(),
                        B == null ? "null" : B.ToString(),
                        R == null ? "null" : R.ToString());
                }
                Console.WriteLine("");
            }

            Console.WriteLine("A AND B AND C");
            foreach (bool? A in states)
            {
                foreach (bool? B in states)
                {
                    foreach (bool? C in states)
                    {
                        var R = A & B & C;
                        Console.WriteLine("{0}\t&\t{1}\t&\t{2}\t= {3}",
                            A == null ? "null" : A.ToString(),
                            B == null ? "null" : B.ToString(),
                            C == null ? "null" : C.ToString(),
                            R == null ? "null" : R.ToString());
                    }
                }
                Console.WriteLine("");
            }

            Console.WriteLine("A OR B");
            foreach (bool? A in states)
            {
                foreach (bool? B in states)
                {
                    var R = A | B;
                    Console.WriteLine("{0}\t|\t{1}\t= {2}",
                        A == null ? "null" : A.ToString(),
                        B == null ? "null" : B.ToString(),
                        R == null ? "null" : R.ToString());
                }
                Console.WriteLine("");
            }

            Console.WriteLine("A OR B OR C");
            foreach (bool? A in states)
            {
                foreach (bool? B in states)
                {
                    foreach (bool? C in states)
                    {
                        var R = A | B | C;
                        Console.WriteLine("{0}\t|\t{1}\t|\t{2}\t= {3}",
                            A == null ? "null" : A.ToString(),
                            B == null ? "null" : B.ToString(),
                            C == null ? "null" : C.ToString(),
                            R == null ? "null" : R.ToString());
                    }
                }
                Console.WriteLine("");
            }

            Console.WriteLine("NOT A");
            foreach (bool? A in states)
            {
                var R = !A;
                Console.WriteLine("!{0}\t= {1}",
                    A == null ? "null" : A.ToString(),
                    R == null ? "null" : R.ToString());
            }

            Console.WriteLine("");

            Console.ReadLine();

            /* Result:
            A AND B
            True    &       True    = True
            True    &       null    = null
            True    &       False   = False

            null    &       True    = null
            null    &       null    = null
            null    &       False   = False

            False   &       True    = False
            False   &       null    = False
            False   &       False   = False

            A AND B AND C
            True    &       True    &       True    = True
            True    &       True    &       null    = null
            True    &       True    &       False   = False
            True    &       null    &       True    = null
            True    &       null    &       null    = null
            True    &       null    &       False   = False
            True    &       False   &       True    = False
            True    &       False   &       null    = False
            True    &       False   &       False   = False

            null    &       True    &       True    = null
            null    &       True    &       null    = null
            null    &       True    &       False   = False
            null    &       null    &       True    = null
            null    &       null    &       null    = null
            null    &       null    &       False   = False
            null    &       False   &       True    = False
            null    &       False   &       null    = False
            null    &       False   &       False   = False

            False   &       True    &       True    = False
            False   &       True    &       null    = False
            False   &       True    &       False   = False
            False   &       null    &       True    = False
            False   &       null    &       null    = False
            False   &       null    &       False   = False
            False   &       False   &       True    = False
            False   &       False   &       null    = False
            False   &       False   &       False   = False

            A OR B
            True    |       True    = True
            True    |       null    = True
            True    |       False   = True

            null    |       True    = True
            null    |       null    = null
            null    |       False   = null

            False   |       True    = True
            False   |       null    = null
            False   |       False   = False

            A OR B OR C
            True    |       True    |       True    = True
            True    |       True    |       null    = True
            True    |       True    |       False   = True
            True    |       null    |       True    = True
            True    |       null    |       null    = True
            True    |       null    |       False   = True
            True    |       False   |       True    = True
            True    |       False   |       null    = True
            True    |       False   |       False   = True

            null    |       True    |       True    = True
            null    |       True    |       null    = True
            null    |       True    |       False   = True
            null    |       null    |       True    = True
            null    |       null    |       null    = null
            null    |       null    |       False   = null
            null    |       False   |       True    = True
            null    |       False   |       null    = null
            null    |       False   |       False   = null

            False   |       True    |       True    = True
            False   |       True    |       null    = True
            False   |       True    |       False   = True
            False   |       null    |       True    = True
            False   |       null    |       null    = null
            False   |       null    |       False   = null
            False   |       False   |       True    = True
            False   |       False   |       null    = null
            False   |       False   |       False   = False

            NOT A
            !True   = False
            !null   = null
            !False  = True
            */
        }
    }

    #endregion
}
