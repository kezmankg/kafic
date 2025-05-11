using System.ComponentModel.DataAnnotations;

namespace Server.Data
{
    public class Article
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public double Price { get; set; }
        public int? SubgroupId { get; set; }
        public virtual Subgroup? Subgroup { get; set; }
    }
}
