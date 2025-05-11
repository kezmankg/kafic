using System.ComponentModel.DataAnnotations;

namespace Server.Data
{
    public class Group
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int? CaffeId { get; set; }
        public virtual Caffe? Caffe { get; set; }
        public virtual IList<Subgroup> Subgroups { get; set; } = new List<Subgroup>();
    }
}
