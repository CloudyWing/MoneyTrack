﻿using System.ComponentModel.DataAnnotations;

namespace CloudyWing.MoneyTrack.Models.Application.IndexModel {
    public class LoginInputModel {
        [Display(Name = "帳號")]
        [Required]
        public string? UserId { get; set; }

        [Display(Name = "密碼")]
        [DataType(DataType.Password)]
        [Required]
        public string? Password { get; set; }
    }
}