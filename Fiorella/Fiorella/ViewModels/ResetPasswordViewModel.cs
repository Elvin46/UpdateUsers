﻿using Fiorella.Models;
using System.ComponentModel.DataAnnotations;

namespace Fiorella.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}
