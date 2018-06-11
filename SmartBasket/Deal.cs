using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SmartBasket
{ 
    public class Deal
    {
    public string Id { get; set; }
    [JsonProperty(PropertyName = "sum")]
     public double sum { get; set; }
    [JsonProperty(PropertyName = "items")]
    public LinkedList<Item> items { get; set; }
}
    public class DealWrapper : Java.Lang.Object
    {
        public DealWrapper(Deal deal)
        {
            Deal = deal;
        }

        public Deal Deal { get; private set; }
    }
}