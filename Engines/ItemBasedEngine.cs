﻿using System;
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
        private int MIN_NUMBER_OF_USERS = 30;
        private double PROBABILITY_THRESHOLD = 0.15;
		private ConcurrentDictionary<Tuple<Item,Item>, int> intersectionCounts;
        private Boolean stopBackgroundThread;
		

        public ItemBasedEngine(Items items){
            this.items = items;
            this.intersectionCounts = new ConcurrentDictionary<Tuple<Item, Item>, int>();

        }


        public List<KeyValuePair<Item, double>> getConditionalProbability(List<Item> givenItems)
        {
            stopBackgroundThread = true;
            return getProbabilities(givenItems, cp);
        }

        public List<KeyValuePair<Item, double>> getJaccardProbability(List<Item> givenItems)
        {
            stopBackgroundThread = true;
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
            await Task.Run(() =>
            {
                foreach (Item itemI in items)
                {
                    if (engineStartedToCompute == true)
                        break;

                    if (itemI.GetRatingUsers().Count < MIN_NUMBER_OF_USERS)
                        continue;

                    Parallel.ForEach(items, (itemJ, loopStateJ) =>
                    {
                        if (!itemI.Equals(itemJ))
                        {
                            if (engineStartedToCompute == true)
                                loopStateJ.Break();

                            if (itemJ.GetRatingUsers().Count < MIN_NUMBER_OF_USERS)
                                return;

                            getIntersectCount(itemI, itemJ);
                            
                        }
                });
            }
         });
        }*/
        public void stopCalculateIntersectionInBackground()
        {
            this.stopBackgroundThread = true;
        }

        public async void calculateIntersectionInBackground()
        {
            await Task.Run(() =>
            {
                foreach (Item itemI in items)
                {
                    if (stopBackgroundThread == true)
                        break;

                    if (itemI.GetRatingUsers().Count < MIN_NUMBER_OF_USERS)
                        continue;

                    foreach (Item itemJ in items)
                    {
                        if (itemI.CompareTo(itemJ)<0)
                        {
                            if (stopBackgroundThread == true)
                                break;

                            if (itemJ.GetRatingUsers().Count < MIN_NUMBER_OF_USERS)
                                return;

                            getIntersectCount(itemI, itemJ);

                        }
                    }
                }
            });
        }
    }
}
