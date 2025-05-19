using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class DiscountModel
    {
        public int Id { get; set; }
        public int? CaffeId { get; set; }
        public string? DeskNo { get; set; }
        public double DiscountPercentage { get; set; }
        public string? UserEmail { get; set; }
    }
}
