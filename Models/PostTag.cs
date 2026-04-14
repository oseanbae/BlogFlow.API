
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogFlow.API.Models
{
    public class PostTag
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;
        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
