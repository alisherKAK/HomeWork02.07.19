using Newtonsoft.Json;

namespace Server
{
    public class Datum
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        [JsonProperty("addressRus")]
        public string AddressRus { get; set; }

        [JsonProperty("addressKaz")]
        public string AddressKaz { get; set; }
    }
}
