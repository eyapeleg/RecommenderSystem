using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem 
{
    public class Stereotype
    {
        private Users users;
        private User centroid;

        public Stereotype(User centroid)
        {
            this.centroid = centroid;
            this.users = new Users();
        }

        public User getCentroid()
        {
            return this.centroid;
        }

        public string GetId()
        {
            return centroid.GetId();
        }

        public void addUser(User user)
        {
            this.users.addUser(user);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Name.Equals(obj.GetType().Name))
                return false;

            if (obj == this)
                return true;

            if (((Stereotype)obj).GetId().Equals(this.GetId()))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return this.GetId().GetHashCode();
        }
    }
}
