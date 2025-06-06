using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class ArticleModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Obavezno polje")]
        [Display(Name = "Naziv podgrupe")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Cena ne sme biti negativan broj")]
        public double Price { get; set; }        
        [Required(ErrorMessage = "Obavezno polje")]
        [Range(1, int.MaxValue, ErrorMessage = "Morate izabrati podgrupu")]
        public int? SubgroupId { get; set; }
        public int? Amount { get; set; }
        public string? ApplicationUserEmail { get; set; }

    }

    public class ArticleModelOrder
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }

        public double TotalPrice => Amount * Price * (100 - Discount) / 100;
        public double Discount { get; set; }
    }

    public class ArticleDiscountModelOrder
    {
        public int ArticleId { get; set; }
        public int OrderId { get; set; }
        public double Discount { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string DeskNo { get; set; } = string.Empty;

    }
}
