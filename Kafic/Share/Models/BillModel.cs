using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class BillModel
    {
        public double Discount { get; set; }
        //Uracunat je popust u TotalPrice
        public double Price { get; set; }
        public string? DeskNo { get; set; }
        public DateTime Date { get; set; }
        public virtual IList<ArticleModelOrder> ArticleModels { get; set; } = new List<ArticleModelOrder>();
    }
}
