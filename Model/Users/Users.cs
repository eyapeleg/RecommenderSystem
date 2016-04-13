using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class Users : IEnumerable<User>
    {
        private Dictionary<string, User> users;

        public Users(){
            users = new Dictionary<string, User>();
        }


        public List<string> GetAllUsers(){
            return users.Keys.ToList();
        }

        public List<string> GetRatedItems(string userId)
        {
            return getUserById(userId).GetRatedItems();
        }

        public User getUserById(string userId)
        {
            User user;
            users.TryGetValue(userId, out user);

            return user;

        }

        public double GetRating(string userId, string itemId)
        {
            User user = getUserById(userId);
            if(user == null)
                return 0.0;       
            
            return user.GetRating(itemId);   
        }

        public void addUser(string userId)
        {
            if (users.Keys.Contains(userId))
                throw new NotSupportedException("Users " + "[" + userId + "]" + " already exists in the DB!");

            User user = new User(userId);
            users.Add(userId, user);
        }

        public void addItemToUser(string userId, string itemId, double rating){
            User user = getUserById(userId);
            
            if (user == null){
                addUser(userId);
                addItemToUser(userId, itemId, rating);
            }
            else{
                user.AddItem(itemId, rating);
            }
        }

        
        public string[] getUsersArray()
        {
            return users.Keys.ToArray();
        }

        public IEnumerator<User> GetEnumerator()
        {
            foreach (KeyValuePair<string,User> u in users)
            {
                yield return u.Value;
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
