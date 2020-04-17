using System.Threading.Tasks;

namespace Broadcast.Core.Infrastructure.Security
{
    public interface IJwtTokenGenerator
    {
        Task<TokenEnvelope> CreateToken(string username);
    }
}