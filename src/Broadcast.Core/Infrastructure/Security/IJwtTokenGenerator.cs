using System.Threading.Tasks;

namespace Broadcast.Core.Infrastructure.Security
{
    public interface IJwtTokenGenerator
    {
        Task<string> CreateToken(string username);
    }
}