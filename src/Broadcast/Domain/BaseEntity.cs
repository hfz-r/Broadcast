using System.ComponentModel.DataAnnotations;

namespace Broadcast.Domain
{
    public abstract class BaseEntity
    {
        [MaxLength(450)]
        public int Id { get; set; }
    }
}