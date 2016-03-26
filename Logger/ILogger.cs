using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public interface ILogger
    {
        void debug(string msg);
        void info(string msg);
    }
}
