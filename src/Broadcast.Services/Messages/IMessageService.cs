using System.Threading.Tasks;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Dtos.Messages;
using Broadcast.Core.Infrastructure.Paging;

namespace Broadcast.Services.Messages
{
    public interface IMessageService
    {
        Task<IPaginate<Message>> FetchMessagesAsync(string tag, string author, int? limit, int? offset);
        Task<Message> InsertMessageAsync(MessageDto dto);
    }
}