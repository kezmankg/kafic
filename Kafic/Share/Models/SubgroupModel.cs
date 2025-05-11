using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class SubgroupModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Obavezno polje")]
        [Display(Name = "Naziv podgrupe")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Obavezno polje")]
        [Range(1, int.MaxValue, ErrorMessage = "Morate izabrati grupu")]
        public int? GroupId { get; set; }
    }
}
