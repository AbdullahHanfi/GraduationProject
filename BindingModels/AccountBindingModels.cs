﻿using System.ComponentModel.DataAnnotations;

namespace GraduationProject.BindingModels
{
    public class LoginBinding {
        [Required(ErrorMessage ="Email Shouldn't be empty")]
        [MaxLength(64)]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password shouldn't be empty")]
        [MaxLength(64)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class RegisterBinding {
        [Required]
        [MaxLength(64)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [MinLength(8)]
        [MaxLength(64)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password mismatch")]
        public string ConfirmPassword { get; set; }
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

    }


}