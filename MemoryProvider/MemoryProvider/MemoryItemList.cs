using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace MonoSoftware.Web.WAO.Providers
{
    /// <summary>
    /// Memory item list.
    /// </summary>
    internal class MemoryItemList : List<MemoryItem>
    {
        /// <summary>
        /// Gets or sets memory item by key.
        /// </summary>
        /// <param name="key">Key of the memory item to get or set</param>
        /// <returns>Memory item</returns>
        public MemoryItem this[string key]
        {
            get
            {
                foreach (MemoryItem item in this)
                {
                    if (item.Key.Equals(key))
                        return item;
                }
                return null;
            }
            set
            {
                MemoryItem itemCH = null;
                foreach (MemoryItem item in this)
                {
                    if (item.Key.Equals(key))
                    {
                        itemCH = item;
                        break;
                    }
                }
                itemCH = value;
            }
        }

        public new void Add(MemoryItem item)
        {
            item.AddedToViewState();
            base.Add(item);
        }

    }
}
