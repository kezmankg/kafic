using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class TurnoverModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public double TotalSum { get; set; }
        public virtual IList<BillModel> BillModels { get; set; } = new List<BillModel>();
    }
}
