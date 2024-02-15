using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
//using Microsoft.Build.Framework;
namespace MvcProject.Models
{
    public class ActionModel
    {
        [Required(ErrorMessage = "Roll no required")]
        public string? rollNo { get; set; }

        [Required(ErrorMessage = "Name required")]
        public string? _name { get; set; }

        [Required(ErrorMessage = "Email required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [DataType(DataType.Password)]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contains atleast one Capital letter and Special character and must be of minimum 8 length")]
        public string? password { get; set; }

        [Required(ErrorMessage = "Confirmation Password is required.")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password Didn't Match")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Phone Number Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Mobile no")]
        public string? mobile { get; set; }

        [Required(ErrorMessage = "Gender Required!")]
        public string? gender { get; set; }

        [Required(ErrorMessage = "Department Required!")]
        public string? dept { get; set; }

    }

    public class ForgotModel
    {
        [Required(ErrorMessage = "Email required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        public string email { get; set; }

        [Required(ErrorMessage ="OTP Required")]
        public string otp { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [DataType(DataType.Password)]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contains atleast one Capital letter and Special character and must be of minimum 8 length")]
        public string password { get; set; }

        [Required(ErrorMessage = "Confirmation Password is required.")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password Didn't Match")]
        public string ConfirmPassword { get; set; }
    }
    public class UpdateModel
    {
        public int roll_No { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? filename { get; set; }
        public string? contentType { get; set; }
        public byte[]? Data { get; set; }
        public string? source { get; set; }
        public string? password { get; set; }
        public string? mobile { get; set; }
        public string? gender { get; set; }
        public string? dept { get; set; }

    }
    public class RegisterModel
    {

        public int roll_No { get; set; }

        [Required(ErrorMessage = "Name required")]
        public string name { get; set; }

        [Required(ErrorMessage = "Email required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        public string email { get; set; }
        public string? password { get; set; }

        [Required(ErrorMessage = "Phone Number Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Mobile no")]
        public string mobile { get; set; }

        [Required(ErrorMessage = "Gender Required!")]
        public string gender { get; set; }

        [Required(ErrorMessage = "Department Required!")]
        public string dept { get; set; }
    }

    public class DropModel
    {
        public int roll_no { get; set; }
    }

    /*public class DropDownDataModel
    {
        public int roll_no { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public long mobile { get; set; }
        public string? gender { get; set;}
        public string? dept { get; set; }

    }
    */
    public class UploadResumeModel
    {
        public int roll_no { get; set; }
        public string? name { get; set; }

        public string? email { get; set; }
        public byte[] File { get; set; }

        public string? filename { get; set; }

    }
    public class HolaModel 
    {
        public int roll_no { get; set; }
        public string? name { get; set; }

        public string? email { get; set; }
    }
    public class FileModel
    {
        public int Id { get; set;}
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}
