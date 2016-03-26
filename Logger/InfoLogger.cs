using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class InfoLogger : ILogger
    {
        public void debug(string msg)
        {
        }

        public void info(string msg)
        {
            Console.Out.WriteLine(msg);
        }
    }
}
