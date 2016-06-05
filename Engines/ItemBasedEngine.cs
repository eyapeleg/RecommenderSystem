using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace RecommenderSystem
{
    public class ItemBasedEngine
    {
        private Items items;
        private int MIN_NUMBER_OF_USERS = 20;
        private double PROBABILITY_THRESHOLD = 0.15;
		private ConcurrentDictionary<Tuple<Item,Item>, int> intersectionCounts;
        private Boolean engineStartedToCompute;
		

        public ItemBasedEngine(Items items){
            this.items = items;
            this.intersectionCounts = new ConcurrentDictionary<Tuple<Item, Item>, int>();

        }


        public List<KeyValuePair<Item, double>> getConditionalProbability(List<Item> givenItems)
        {
            engineStartedToCompute = true;
            return getProbabilities(givenItems, cp);
        }

        public List<KeyValuePair<Item, double>> getJaccardProbability(List<Item> givenItems)
        {
            engineStartedToCompute = true;
            return getProbabilities(givenItems, jaccard);
        }

        private List<KeyValuePair<Item, double>> getProbabilities(List<Item> givenItems, Func<Item,Item,double> probabilityFunction)
        {
            List<KeyValuePair<Item, double>> probabilities =
            items.AsParallel()
                .Where(item => itemValid(givenItems, item))
                .Select(item =>{
                    double itemProbability = calculateProbability(givenItems, probabilityFunction, item);
                    return new KeyValuePair<Item,double>(item, itemProbability);})
                //.Where(kv => kv.Value > PROBABILITY_THRESHOLD)
                .ToList();

            return probabilities
                .OrderByDescending(kv => kv.Value)
                .ToList();
        }

        private double calculateProbability(List<Item> givenItems, Func<Item, Item, double> probabilityFunction, Item item)
        {
            return givenItems
                .Select(givenItem => probabilityFunction(item, givenItem))
                .Max();
         }

        private bool itemValid(List<Item> givenItems, Item item)
        {
            return item.GetRatingUsers().Count() > MIN_NUMBER_OF_USERS && !givenItems.Contains(item);
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
				intersectionCounts.TryAdd(itemTuple, count);
			}
			
			return intersectionCounts[itemTuple];
		}

        /*public async void calculateIntersectionInBackground()
        {
            Parallel.ForEach(items, (itemI, loopStateI)=>
            {
                if (engineStartedToCompute == true)
                    loopStateI.Break();

                Parallel.ForEach(items, (itemJ,loopStateJ) =>
                {
                    if (!itemI.Equals(itemJ))
                    {
                        getIntersectCount(itemI, itemJ);
                        if (engineStartedToCompute == true)
                            loopStateJ.Break();
                    }
                });
            });
        }*/

        public async void calculateIntersectionInBackground()
        {
            await Task.Run(() =>
            {
                foreach (Item itemI in items)
                {
                    if (engineStartedToCompute == true)
                        break;

                    Parallel.ForEach(items, (itemJ, loopStateJ) =>
                    {
                        if (!itemI.Equals(itemJ))
                        {
                            getIntersectCount(itemI, itemJ);
                            if (engineStartedToCompute == true)
                                loopStateJ.Break();
                        }
                });
            }
         });
        }    
    }
}
