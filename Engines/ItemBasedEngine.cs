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
		private Dictionary<Tuple<Item,Item>, int> intersectionCounts;
		

        public ItemBasedEngine(Items items){
            this.items = items;
			this.intersectionCounts = new Dictionary<Tuple<Item,Item>, int>();
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


        private double cp(Item item, Item givenItem) 
        {
            int MIN_NUMBER_OF_USERS = 10;
            List<string> itemUseres = item.GetRatingUsers();
            List<string> givenItemUsers = givenItem.GetRatingUsers();

            int numerator = getIntersectCount(item, givenItem);
            int denominator = givenItemUsers.Count();

            if (denominator == 0 || numerator < MIN_NUMBER_OF_USERS)
                return 0.0;

            return (double)numerator / (double)denominator;
        }

        private double jaccard(Item item, Item givenItem)
        {
            int MIN_NUMBER_OF_USERS = 10;
            List<string> itemUseres = item.GetRatingUsers();
            List<string> givenItemUsers = givenItem.GetRatingUsers();

            int numerator = getIntersectCount(item, givenItem);
            int denominator = givenItemUsers.Count()+itemUseres.Count()-numerator;

            if (denominator == 0 || numerator < MIN_NUMBER_OF_USERS)
                return 0.0;

            return (double)numerator / (double)denominator;
        }
		
		private int getIntersectCount(Item item1, Item item2){
			Tuple<Item, Item> itemTuple;
			
			if (item1.GetId().CompareTo(item2.GetId())<0){
				itemTuple = Tuple.Create(item1,item2);
			}
			else{
				itemTuple = Tuple.Create(item2,item1);
			}
			
			if (!intersectionCounts.ContainsKey(itemTuple)){
				List<string> item1Users = item1.GetRatingUsers();
				List<string> item2Users = item2.GetRatingUsers();
				
				int count = item1Users.Intersect(item2Users).Count();
				intersectionCounts.Add(itemTuple, count);
			}
			
			return intersectionCounts[itemTuple];
		}

    }
}
