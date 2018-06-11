using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SmartBasket
{
 
    public class Item
    {
        public string Id { get; set; }
        [JsonProperty(PropertyName = "itemId")]
        public string itemId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }
        [JsonProperty(PropertyName = "price")]
        public string price { get; set; }


    }

    public class ItemWrapper : Java.Lang.Object
    {
        public ItemWrapper(Item item)
        {
            Item = item;
        }

        public Item Item { get; private set; }
    }
}