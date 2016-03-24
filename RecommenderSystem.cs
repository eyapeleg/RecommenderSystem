﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class RecommenderSystem
    {
        public enum PredictionMethod { Pearson, Cosine, Random };
        private Users users;
        private Items items;

        //class members here

        //constructor
        public RecommenderSystem()
        {
            users = new Users();
            items = new Items();
        }

        public void Load(string sFileName)
        {
            DataLoader dataLoader = new DataLoader();
            Tuple<Users,Items> data = dataLoader.Load(sFileName);
            users = data.Item1;
            items = data.Item2;
        }

        //return a list of the ids of all the users in the dataset
        public List<string> GetAllUsers()
        {
            return users.GetAllUsers();
        }

        //returns a list of all the items in the dataset
        public List<string> GetAllItems()
        {
            return items.GetAllItems();
        }

        //returns the list of all items that the given user has rated in the dataset
        public List<string> GetRatedItems(string sUID)
        {
            return users.GetRatedItems(sUID);
        }

        //Returns a user-item rating that appears in the dataset (not predicted)
        public double GetRating(string sUID, string sIID)
        {
            return users.GetRating(sUID, sIID);
        }

        //predict a rating for a user item pair using the specified method
        public double PredictRating(PredictionMethod m, string sUID, string sIID)
        {
            throw new NotImplementedException();
        }

        //Compute MAE (mean absolute error) for a set of rating prediction methods over the same user-item pairs
        //cTrials specifies the number of user-item pairs to be tested
        public Dictionary<PredictionMethod, double> ComputeMAE(List<PredictionMethod> lMethods, int cTrials)
        {
            throw new NotImplementedException();
        }
    }
}
