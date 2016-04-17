using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace RecommenderSystem 
{
    public class Stereotype
    {
        private Users users;
        private User centroid;
        private static int nextId=1;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Stereotype(User user)
        {
            User newCentroid = new User("Stereotype#" + nextId);
            nextId++;
            foreach(string itemId in user.GetRatedItems()){
                newCentroid.AddItemById(itemId, user.GetRating(itemId));
            }

            this.centroid = newCentroid;
            this.users = new Users();        
        }

        public Users getUsers()
        {
            return new Users(this.users);
        }

        public Stereotype(Stereotype stereotype)
        {
            this.users = new Users(stereotype.users);
            this.centroid = new User(stereotype.centroid);
        }
        
        public User getCentroid()
        {
            return new User(this.centroid);
        }

        public string GetId()
        {
            return centroid.GetId();
        }

        public void addUser(User user)
        {
            this.users.addUser(new User(user));
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

        public void reCalculateCentroid()
        {
            foreach (string itemId in centroid.GetRatedItems())
            {
                double sum = 0.0;
                int n = 0;
                
                foreach (User user in users)
                {
                    if (user.GetRating(itemId) != 0.0)
                    {
                        sum += user.GetRating(itemId);
                        n++;
                    }
                }

                if (n!=0)
                    centroid.SetRating(itemId, sum / (double)n);
            }
        }

        public void initUsers()
        {
            this.users = new Users();
        }
    }
}
