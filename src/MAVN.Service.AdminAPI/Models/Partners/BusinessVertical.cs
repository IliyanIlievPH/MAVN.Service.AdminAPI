using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.Partners
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BusinessVertical
    {
        Hospitality,
        RealEstate,
        Retail
    }
}
