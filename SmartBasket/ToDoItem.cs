using System;
using Newtonsoft.Json;

namespace SmartBasket
{
	public class ToDoItem
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "text")]
		public string Text { get; set; }

        [JsonProperty(PropertyName = "price")]
        public int price { get; set; }

        [JsonProperty(PropertyName = "complete")]
		public bool Complete { get; set; }
	}

	public class ToDoItemWrapper : Java.Lang.Object
	{
		public ToDoItemWrapper (ToDoItem item)
		{
			ToDoItem = item;
		}

		public ToDoItem ToDoItem { get; private set; }
	}
}

