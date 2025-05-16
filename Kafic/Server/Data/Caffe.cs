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
        public virtual IList<Group> Groups { get; set; } = new List<Group>();
        public virtual IList<Order> Orders { get; set; } = new List<Order>();
        public virtual IList<OrderPaid> OrderPaids { get; set; } = new List<OrderPaid>();
        public virtual IList<Bill> Bills { get; set; } = new List<Bill>();
    }
}
