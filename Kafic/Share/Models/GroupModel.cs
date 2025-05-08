using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class GroupModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Obavezno polje")]
        [Display(Name = "Naziv grupe")]
        public string Name { get; set; } = string.Empty;
        public int? CaffeId { get; set; }
        public string AdminEmail { get; set; } = string.Empty;
    }
}
