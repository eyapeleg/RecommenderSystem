using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string s)
            : base(s)
        {
        }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string s)
            : base(s)
        {
            
        }
    }
}
