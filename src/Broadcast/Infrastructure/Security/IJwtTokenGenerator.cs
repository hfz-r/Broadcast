using System.Threading.Tasks;

namespace Broadcast.Infrastructure.Security
{
    public interface IJwtTokenGenerator
    {
        Task<string> CreateToken(string username);
    }
}