using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class ItemBasedEngine
    {
        private Items items;
        private int MIN_NUMBER_OF_USERS = 10;

        public ItemBasedEngine(Items items){
            this.items = items;
        }


        public List<KeyValuePair<Item, double>> getConditionalProbability(List<Item> givenItems)
        {
            return getProbabilities(givenItems, cp);
        }

        public List<KeyValuePair<Item, double>> getJaccardProbability(List<Item> givenItems)
        {
            return getProbabilities(givenItems, jaccard);
        }

        private List<KeyValuePair<Item, double>> getProbabilities(List<Item> givenItems, Func<Item,Item,double> probabilityFunction)
        {
            List<KeyValuePair<Item, double>> probabilities = new List<KeyValuePair<Item, double>>();
            foreach (Item item in items)
            {
                if (itemNotValid(givenItems, item))
                {
                    continue;
                }

                double itemProbability = calculateProbability(givenItems, probabilityFunction, item);
                probabilities.Add(new KeyValuePair<Item, double>(item, itemProbability));
            }
            return probabilities.OrderByDescending(kv => kv.Value).ToList();
        }

        private double calculateProbability(List<Item> givenItems, Func<Item, Item, double> probabilityFunction, Item item)
        {
            return givenItems
                .Select(givenItem => probabilityFunction(item, givenItem))
                .Max();
         }

        private bool itemNotValid(List<Item> givenItems, Item item)
        {
            return item.GetRatingUsers().Count() < MIN_NUMBER_OF_USERS || givenItems.Contains(item);
        }


        Func<Item, Item, double> cp = (Item item, Item givenItem) =>
        {
            List<string> itemUseres = item.GetRatingUsers();
            List<string> givenItemUsers = givenItem.GetRatingUsers();

            int numerator = itemUseres.Intersect(givenItemUsers).Count();
            int denominator = givenItemUsers.Count();

            if (denominator == 0 || numerator == 0)
                return 0.0;

            return (double)numerator / (double)denominator;
        };

        Func<Item,Item,double> jaccard = (Item item, Item givenItem) =>
        {
            List<string> itemUseres = item.GetRatingUsers();
            List<string> givenItemUsers = givenItem.GetRatingUsers();

            int numerator = itemUseres.Intersect(givenItemUsers).Count();
            int denominator = givenItemUsers.Count()+itemUseres.Count()-numerator;

            if (denominator == 0 || numerator == 0)
                return 0.0;

            return (double)numerator / (double)denominator;
        };

    }
}
