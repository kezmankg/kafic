using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public int? CaffeId { get; set; }
        public string? DeskNo { get; set; }
        public DateTime Date { get; set; }
        public virtual IList<ArticleModel> ArticleModels { get; set; } = new List<ArticleModel>();
        public string? ApplicationUserEmail { get; set; }
    }
}
