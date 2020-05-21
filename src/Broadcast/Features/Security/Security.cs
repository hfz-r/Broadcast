using System.Collections.Generic;
using Broadcast.Core.Dtos.Security;
using Newtonsoft.Json;

namespace Broadcast.Features.Security
{
    public class SecurityEnvelope
    {
        public SecurityEnvelope(SecurityDto security)
        {
            Security = security;
        }

        [JsonProperty("security")]
        public SecurityDto Security { get; set; }
    }

    public class SecuritiesEnvelope
    {
        [JsonProperty("securities")]
        public List<SecurityDto> Securities { get; set; }

        [JsonProperty("securitiesCount")]
        public int SecuritiesCount { get; set; }
    }
}