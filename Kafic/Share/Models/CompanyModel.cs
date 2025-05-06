using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class CompanyModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Obavezno polje")]
        [Display(Name = "Ime Kafica")]
        public string Name { get; set; } = string.Empty;
        [Range(0, 1000, ErrorMessage = "Broj stolova biti veci ili jednak nuli")]
        public int TablesNo { get; set; }
        [Range(0, 1000, ErrorMessage = "Broj lezaljki biti veci ili jednak nuli")]
        public int SunLoungersNo { get; set; }
    }
}
