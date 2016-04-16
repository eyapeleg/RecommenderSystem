using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text   ;

namespace RecommenderSystem
{
    public class Stereotypes :IEnumerable<Stereotype>
    {
        private Dictionary<string,Stereotype> stereotypes;

        public Stereotypes()
        {
            this.stereotypes = new Dictionary<string,Stereotype>();
        }
        
        public void  addStereotype(Stereotype stereotype){
            stereotypes.Add(stereotype.GetId(),stereotype);
        }

        public Stereotype getSeterotype(string stereotypeId)
        {
            return stereotypes[stereotypeId];
        }

        public Dictionary<string,Stereotype> getStereotypes()
        {
            return stereotypes;
        }

        public List<User> getStereotypesCentroids()
        {
            return stereotypes.Select(s => s.Value.getCentroid()).ToList();
        }

        public IEnumerator<Stereotype> GetEnumerator()
        {
            return stereotypes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void addUserToStereotype(string stereotypeId, User user)
        {
            stereotypes[stereotypeId].addUser(user);

        }


    }
}
