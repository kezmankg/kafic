using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class PayOrderModel
    {
        public string DescNo { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public double TotalSum { get; set; }
    }
}
