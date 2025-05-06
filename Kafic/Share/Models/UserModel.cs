using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Models
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Obavezno polje")]
        [EmailAddress(ErrorMessage = "Email nije validan")]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; } = string.Empty;
        [Required(ErrorMessage = "Obavezno polje")]
        [DataType(DataType.Password)]
        [StringLength(15, ErrorMessage = "Sifra mora imati najmanje 3 karaktera, a maksimalno 15 karaktera", MinimumLength = 3)]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Obavezno polje")]
        [Display(Name = "Ime i prezime")]
        public string FullName { get; set; } = string.Empty;
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obavezno polje")]
        [Display(Name = "Ime Kafica")]
        public string CaffeName { get; set; } = string.Empty;
        [Range(0, 1000, ErrorMessage = "Broj stolova biti veci ili jednak nuli")]
        public int TablesNo { get; set; }
        [Range(0, 1000, ErrorMessage = "Broj lezaljki biti veci ili jednak nuli")]
        public int SunLoungersNo { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Obavezno polje")]
        [EmailAddress(ErrorMessage = "Email nije validan")]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; } = string.Empty;
        [Required(ErrorMessage = "Obavezno polje")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
    }
}
