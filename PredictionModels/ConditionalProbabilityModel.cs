using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    class ConditionalProbabilityModel 
    {
        private int PairAppearanceThresholdCount = 10;
        private ItemToItem itemToItem;
        private Users users;
        private Items items;

        public ConditionalProbabilityModel(Users users, Items items)
        {
            this.users = users;
            this.items = items;
        }

        public List<KeyValuePair<Item, double>> Recommend(Item givenItem, Items items)
        {
            return items
                .Where(item => !item.Equals(givenItem))
                .Select(item => new KeyValuePair<Item, double>(item, itemToItem.getConditionalProbability(givenItem, item)))
                .OrderBy(kv => kv.Value)
                .ToList();
        }

        public List<KeyValuePair<Item, double>> Recommend(List<Item> givenItems, Items items)
        {
            List<KeyValuePair<Item, double>> conditionalProbabilities = new List<KeyValuePair<Item, double>>();
            foreach (Item item in items)
            {
                double maxProbability = givenItems
                                            .Select(givenItem => itemToItem.getConditionalProbability(givenItem, item))
                                            .Max();

                conditionalProbabilities.Add(new KeyValuePair<Item,double>(item, maxProbability));
            }
            return conditionalProbabilities;
        }

        public void Train()
        {
            ItemToItem itemToitem = extractItemPairsFromUsers();
            itemToitem = removePairsWithLowMutualCount(itemToitem);
            this.itemToItem = itemToitem;
        }

        private ItemToItem extractItemPairsFromUsers()
        {
            ItemToItem itemToItem = new ItemToItem();

            foreach (User user in users)
            {
                List<Tuple<Item, Item>> itemPairs = getAllItemsPairs(user);
                foreach (Tuple<Item, Item> itemPair in itemPairs)
                {
                    itemToItem.addPair(itemPair);
                }
            }
            return itemToItem;
        }

        private List<Tuple<Item, Item>> getAllItemsPairs(User user)
        {
            var userItems = user.GetRatedItems().Select(itemId => items.GetItemById(itemId));
            List<Tuple<Item, Item>> itemPairs = DataUtils.getAllPairedCombinations(userItems);
            return itemPairs;
        }

        private ItemToItem removePairsWithLowMutualCount(ItemToItem itemToItem)
        {
            List<Tuple<Item, Item>> itemPairsToRemove = itemToItem
                                                        .Where(itemToItemCount => itemToItemCount.Value < PairAppearanceThresholdCount)
                                                        .Select(kv => kv.Key)
                                                        .ToList();

            foreach (Tuple<Item, Item> itemPair in itemPairsToRemove)
            {
                itemToItem.removePair(itemPair);
            }

            return itemToItem;
        }
    }
}
