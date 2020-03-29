using System.ComponentModel.DataAnnotations;

namespace Broadcast.Core.Domain
{
    public abstract class BaseEntity
    {
        [MaxLength(450)]
        public int Id { get; set; }
    }
}