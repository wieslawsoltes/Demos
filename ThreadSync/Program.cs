#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#endregion

namespace ThreadSync
{
    #region Program

    public class Data
    {
        public int Count { get; set; }
    }

    class Program
    {
        private static void Main(string[] args)
        {
            Run();
        }

        private static Action<Data, Data> Copy = (src, dst) =>
        {
            dst.Count = src.Count;
        };

        private static void Run()
        {
            var data = new Data() { Count = 0 };
            var service = CreateService();
            string line;

            while (true)
            {
                // get event from user
                line = Console.ReadLine();
                data.Count++;
                Console.Title = data.Count.ToString();

                // check for quit command
                if (line.ToLower() == "q")
                {
                    service.Stop();
                    Console.WriteLine("Quit");
                    break;
                }

                // try to handle next frame
                var result = service.HandleEvent(data, Copy, 16);
                if (result == false)
                    Console.WriteLine("Skip: " + data.Count);
            }
        }

        private static BackgroundService<Data> CreateService()
        {
            var service = new BackgroundService<Data>();

            service.Start((data) =>
            {
                // draw frame
                Thread.Sleep(160);

                Console.WriteLine("Count: " + data.Count);
            },
            new Data());

            return service;
        }
    } 

    #endregion
}
