using System;
using Newtonsoft.Json;

namespace Broadcast.Core.Infrastructure.Security
{
    public class TokenEnvelope
    {
        public TokenEnvelope(string token, DateTime expiresAt)
        {
            Token = token;
            ExpiresAt = expiresAt;
        }

        [JsonProperty("token")]
        public string Token { get; }

        [JsonProperty("expiresAt")]
        public DateTime ExpiresAt { get; }
    }
}