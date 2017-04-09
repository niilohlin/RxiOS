using System;
using System.Security.Policy;
using Newtonsoft.Json;

namespace RxiOSExample.Models
{
    public class User
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("avatar_url")]
        public Uri AvatarUri { get; set; }

        [JsonProperty("followers_url")]
        public Uri FollowersUri { get; set; }

        [JsonProperty("following_url")]
        public Uri FollowingUri { get; set; }
    }
}