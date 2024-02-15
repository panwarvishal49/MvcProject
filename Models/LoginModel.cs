using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
namespace MvcProject.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Enter Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [DataType(DataType.Password)]
        public string? password { get; set; }
    }
}
