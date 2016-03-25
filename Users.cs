using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class Users
    {
        private ISet<User> users;

        public Users(){
            users = new HashSet<User>();
        }


        public List<string> GetAllUsers(){
            return users.ToList().ConvertAll<string>(x => x.getId()).ToList();
        }

        public List<string> GetRatedItems(string userId)
        {
            return getUserById(userId).getRatedItems();
        }

        public User getUserById(string userId)
        {
            List<User> usersIdsList = users.Where(x => (x.getId()==userId)).ToList();
            return usersIdsList.FirstOrDefault();
        }

        public double GetRating(string userId, string itemId)
        {
            User user = getUserById(userId);
            if(user == null)
                return 0.0;       
            
            return user.getRating(itemId);   
        }

        public void addUser(string userId)
        {
            User user = new User(userId);
            if (users.Contains(user))
                throw new NotSupportedException("Users " + "[" + userId + "]" + " already exists in the DB!");

            users.Add(user);
        }

        public void addItemToUser(string userId, string itemId, double rating){
            User user = getUserById(userId);
            
            if (user == null){
                addUser(userId);
                addItemToUser(userId, itemId, rating);
            }
            else{
                user.addItem(itemId, rating);
            }
        }

    
        

    }
}
