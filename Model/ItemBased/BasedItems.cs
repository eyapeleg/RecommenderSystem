using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class BasedItems : IEnumerable<BasedItem>
    {
        private Dictionary<Item, BasedItem> basedItems;
        private Dictionary<Item, int> itemsCount;

        public BasedItems()
        {
            basedItems = new Dictionary<Item, BasedItem>();
        }

        public void addItem(Item itemToAdd)
        {
            if (itemsCount.ContainsKey(itemToAdd))
            {
                itemsCount[itemToAdd]++;
            }
            else
            {
                itemsCount.Add(itemToAdd,1);
            }
        }

        public void addItem(Item keyItem, Item itemToAdd)
        {
            if(basedItems.ContainsKey(keyItem)){
                basedItems[keyItem].addItem(itemToAdd);
            }
            else
            {
                basedItems.Add(keyItem, new BasedItem());
                basedItems[keyItem].addItem(itemToAdd);
            }
        }

        public double getConditionalProbability(Item item)
        {
            return basedItems
                        .Values
                        .Select(basedItem => basedItem.getConditionalProbability(item))
                        .Max();
        }


        public IEnumerator<BasedItem> GetEnumerator()
        {
            return basedItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
