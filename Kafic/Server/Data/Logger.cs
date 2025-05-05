using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Data
{
    public class Logger
    {
        public int Id { get; set; }
        [Required]
        public string Time { get; set; } = string.Empty;
        [Required]
        //Error, info, warning,...
        public string Level { get; set; } = string.Empty;
        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

}
