using System.ComponentModel.DataAnnotations;

namespace Server.Data
{
    public class Caffe
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int TablesNo { get; set; }
        public int SunLoungersNo { get; set; }
        public virtual IList<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
    }
}
