using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class StereotypesEngine
    {
        private RandomGenerator randomGenerator;

        public StereotypesEngine(int cStereotypes)
        {
            this.randomGenerator = new RandomGenerator();
        }

        public void initStereotypes(Users users, SimilarityEngine similarityEngine, int cStereotypes)
        {
            //TODO create a list of candidate users
            for (int i = 0; i < cStereotypes; i++)
            {
                User stereotypeUser = randomGenerator.newRandomUser(users);
                //TODO select a candidate user
            }

            
        }
    }
}
