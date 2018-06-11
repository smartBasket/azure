using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;


namespace SmartBasket
{

    public class User
    {
        public string Id { get; set; }
        [JsonProperty(PropertyName = "userId")]
        public string userId { get; set; }
        [JsonProperty(PropertyName = "password")]
        public string password { get; set; }
        [JsonProperty(PropertyName = "deals")]
        public LinkedList<Deal> deals { get; set; }



    }

    public class UserWrapper : Java.Lang.Object
    {
        public UserWrapper(User user)
        {
            User = user;
        }

        public User User { get; private set; }
    }
}