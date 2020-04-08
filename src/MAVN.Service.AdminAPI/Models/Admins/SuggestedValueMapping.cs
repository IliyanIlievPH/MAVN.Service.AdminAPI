using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.Admins
{
    /// <summary>
    /// Represents a mapping for a suggested type
    /// </summary>
    public class SuggestedValueMapping
    {
        /// <summary>
        /// Type of information
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SuggestedValueType Type { set; get; }
        
        /// <summary>
        /// Possible values
        /// </summary>
        public List<string> Values { set; get; }
    }
}
