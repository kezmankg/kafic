using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public int? CaffeId { get; set; }
        [Required(ErrorMessage = "Obavezno polje")]
        [Display(Name = "Broj stola")]
        public string? DeskNo { get; set; }
        public DateTime Date { get; set; }
        public virtual IList<ArticleModelOrder> ArticleModels { get; set; } = new List<ArticleModelOrder>();
        public string? ApplicationUserEmail { get; set; }

        public double TotalOrderPrice => ArticleModels.Sum(a => a.TotalPrice);
    }
}
