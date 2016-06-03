using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    class ConditionalProbabilityModel 
    {
        private int PairAppearanceThresholdCount = 10;
        private Users users;
        private Items items;

        public ConditionalProbabilityModel(Users users, Items items)
        {
            this.users = users;
            this.items = items;
        }

        public List<KeyValuePair<Item, double>> Recommend(User userToRecommendTo)
        {
            BasedItems basedItems = createBasedItemsFromAllUsers(userToRecommendTo);
           // basedItems = calculateTotalItemsCountFromAllUsers(basedItems, userToRecommendTo);
            basedItems = removePairsWithLowMutualCount(basedItems);
            return calculateConditionalProbabilities(userToRecommendTo, basedItems);
        }

        /*private BasedItems calculateTotalItemsCountFromAllUsers(BasedItems basedItems, User userToRecommendTo)
        {
            foreach (User user in users)
            {
                if (user.Equals(userToRecommendTo))
                    continue;

                List<Item> userItems = user.GetRatedItems().Select(itemId => items.GetItemById(itemId)).ToList();
                foreach (Item item in userItems)
                {
                    //bas
                }

            }
        }*/

        private BasedItems createBasedItemsFromAllUsers(User userToRecommendTo)
        {
            BasedItems basedItems = new BasedItems();
            foreach (User userToRecommendFrom in users)
            {
                if (userToRecommendFrom.Equals(userToRecommendTo))
                    continue;

                basedItems = createBasedItemsFromUser(basedItems, userToRecommendFrom, userToRecommendTo);
            }
            return basedItems;
        }

        private BasedItems createBasedItemsFromUser(BasedItems basedItems, User userToRecommendFrom, User userToRecommendTo)
        {
            List<Item> destUserItems = userToRecommendFrom.GetRatedItems().Select(itemId => items.GetItemById(itemId)).ToList();
            List<Item> baseUserItems = userToRecommendTo.GetRatedItems().Select(itemId => items.GetItemById(itemId)).ToList();
            foreach (Item baseUserItem in baseUserItems)
            {
                basedItems = extractItemSpecificPairs(basedItems, destUserItems, baseUserItems, baseUserItem);
            }
            return basedItems;
        }

        private BasedItems extractItemSpecificPairs(BasedItems basedItems, List<Item> destUserItems, List<Item> baseUserItems, Item baseUserItem)
        {
            if (destUserItems.Contains(baseUserItem))
            {
                foreach (Item destUserItem in destUserItems)
                {
                    if (!baseUserItems.Contains(destUserItem))
                    {
                        basedItems.addItem(baseUserItem, destUserItem);
                    }
                }
            }
            return basedItems;
        }

        private BasedItems removePairsWithLowMutualCount(BasedItems basedItems)
        {
            foreach (BasedItem basedItem in basedItems)
            {
                List<Item> itemPairsToRemove = new List<Item>();
                foreach (KeyValuePair<Item, int> itemCount in basedItem)
                {
                    if (itemCount.Value < PairAppearanceThresholdCount)
                    {
                        itemPairsToRemove.Add(itemCount.Key);
                    }
                }
                foreach (Item item in itemPairsToRemove)
                {
                    basedItem.removeItem(item);
                }
            }

            return basedItems;
        }

        private List<KeyValuePair<Item, double>> calculateConditionalProbabilities(User userToRecommendTo, BasedItems basedItems)
        {
            List<Item> userItems = userToRecommendTo.GetRatedItems().Select(itemId => items.GetItemById(itemId)).ToList();
            List<KeyValuePair<Item, double>> conditionalProbabilities = new List<KeyValuePair<Item, double>>();
            
            foreach (Item item in items)
            {
                if (userItems.Contains(item))
                    continue;

                double maxProbability = basedItems.getConditionalProbability(item);
                conditionalProbabilities.Add(new KeyValuePair<Item, double>(item, maxProbability));
            }
            return conditionalProbabilities.OrderByDescending(kv => kv.Value).ToList();
        }


 
    }
}
