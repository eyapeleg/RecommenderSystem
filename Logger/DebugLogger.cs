using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class DebugLogger : ILogger
    {
        public void debug(string msg)
        {
            Console.Out.WriteLine(msg);
        }

        public void info(string msg)
        {
            Console.Out.WriteLine(msg);
        }
    }
}
