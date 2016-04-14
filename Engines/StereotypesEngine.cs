using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class StereotypesEngine
    {
        private Dictionary<int, List<User>> stereotypesGroups;
        private int cStereotypes;

        public StereotypesEngine(int cStereotypes)
        {
            this.cStereotypes = cStereotypes;
            this.stereotypesGroups = new Dictionary<int, List<User>>();

            //Intialize dictionary and set their Id
            for (int i = 1; i <= cStereotypes ; i++)
            {
                stereotypesGroups.Add(i, new List<User>());
            }


        }
    }
}
