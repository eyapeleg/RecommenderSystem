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
            ItemToItems itemsToItems = createItemToItemFromUser(userToRecommendTo);
            itemsToItems = removePairsWithLowMutualCount(itemsToItems);
            return calculateConditionalProbabilities(userToRecommendTo, itemsToItems);
        }

        private ItemToItems createItemToItemFromUser(User userToRecommendTo)
        {
            ItemToItems itemsToItems = new ItemToItems();
            foreach (User userToRecommendFrom in users)
            {
                if (userToRecommendFrom.Equals(userToRecommendTo))
                    continue;

                itemsToItems = extractAllPairsFromUser(itemsToItems, userToRecommendFrom, userToRecommendTo);
            }
            return itemsToItems;
        }

        private ItemToItems extractAllPairsFromUser(ItemToItems itemsToItems, User userToRecommendFrom, User userToRecommendTo)
        {
            List<Item> destUserItems = userToRecommendFrom.GetRatedItems().Select(itemId => items.GetItemById(itemId)).ToList();
            List<Item> baseUserItems = userToRecommendTo.GetRatedItems().Select(itemId => items.GetItemById(itemId)).ToList();
            foreach (Item baseUserItem in baseUserItems)
            {
                itemsToItems = extractItemSpecificPairs(itemsToItems, destUserItems, baseUserItems, baseUserItem);
            }
            return itemsToItems;
        }

        private ItemToItems extractItemSpecificPairs(ItemToItems itemsToItems, List<Item> destUserItems, List<Item> baseUserItems, Item baseUserItem)
        {
            if (destUserItems.Contains(baseUserItem))
            {
                foreach (Item destUserItem in destUserItems)
                {
                    if (!baseUserItems.Contains(destUserItem))
                    {
                        itemsToItems.addItem(baseUserItem, destUserItem);
                    }
                }
            }
            return itemsToItems;
        }

        private ItemToItems removePairsWithLowMutualCount(ItemToItems itemToItems)
        {
            foreach (ItemToItem itemToItem in itemToItems)
            {
                List<Item> itemPairsToRemove = new List<Item>();
                foreach (KeyValuePair<Item, int> itemCount in itemToItem)
                {
                    if (itemCount.Value < PairAppearanceThresholdCount)
                    {
                        itemPairsToRemove.Add(itemCount.Key);
                    }
                }
                foreach (Item item in itemPairsToRemove)
                {
                    itemToItem.removeItem(item);
                }
            }

            return itemToItems;
        }

        private List<KeyValuePair<Item, double>> calculateConditionalProbabilities(User userToRecommendTo, ItemToItems itemsToItems)
        {
            List<Item> userItems = userToRecommendTo.GetRatedItems().Select(itemId => items.GetItemById(itemId)).ToList();
            List<KeyValuePair<Item, double>> conditionalProbabilities = new List<KeyValuePair<Item, double>>();
            
            foreach (Item item in items)
            {
                if (userItems.Contains(item))
                    continue;

                double maxProbability = itemsToItems
                                            .Select(itemToItem => itemToItem.getConditionalProbability(item))
                                            .Max();

                conditionalProbabilities.Add(new KeyValuePair<Item, double>(item, maxProbability));
            }
            return conditionalProbabilities.OrderByDescending(kv => kv.Value).ToList();
        }


 
    }
}
