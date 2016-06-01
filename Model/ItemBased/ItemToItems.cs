using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class ItemToItems : IEnumerable<ItemToItem>
    {
        private Dictionary<Item, ItemToItem> itemToItems;

        public ItemToItems()
        {
            itemToItems = new Dictionary<Item, ItemToItem>();
        }

        public void addItem(Item keyItem, Item itemToAdd)
        {
            if(itemToItems.ContainsKey(keyItem)){
                itemToItems[keyItem].addItem(itemToAdd);
            }
            else
            {
                itemToItems.Add(keyItem, new ItemToItem());
                itemToItems[keyItem].addItem(itemToAdd);
            }
        }

        /*public double getConditionalProbability(Item keyItem, Item item)
        {
            if (itemToItems.ContainsKey(keyItem))
            {
                return itemToItems[keyItem].getConditionalProbability(item);
            }
            return 0.0;
        }*/


        public IEnumerator<ItemToItem> GetEnumerator()
        {
            return itemToItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
