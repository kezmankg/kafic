using Share.Models;
using System.ComponentModel.DataAnnotations;

namespace Server.Data
{
    public class Subgroup
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int? GroupId { get; set; }
        public virtual Group? Group { get; set; }
        public virtual IList<Article> Articles { get; set; } = new List<Article>();
    }
}
