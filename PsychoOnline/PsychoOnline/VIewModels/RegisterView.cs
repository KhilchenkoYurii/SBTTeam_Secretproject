using System;
using System.ComponentModel.DataAnnotations;

namespace PsychoWeb.ViewModels
{
    public class RegisterView
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "SecondName")]
        public string SecondName { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Compare("Email", ErrorMessage = "Emails don't match!")]
        [Display(Name = "EmailConfirm")]
        public string EmailConfirm { get; set; }

        public string inform { get; set; }

        [Required]
        [Display(Name = "BirthDate")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public string Genre { get; set; }

        public decimal Age { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords don't match!")]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordConfirm")]
        public string PasswordConfirm { get; set; }
    }
}
