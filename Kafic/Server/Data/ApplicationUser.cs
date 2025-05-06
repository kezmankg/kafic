using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Server.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FullName { get; set; } = string.Empty;

        public int? CaffeId { get; set; }
        public virtual Caffe? Caffe { get; set; }
    }
}
