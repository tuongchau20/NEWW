﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthWind.Models.account
{
    public class ApplicationUser : IdentityUser
    {
        [Column("FirstName")]
        public string? FirstName { get; set; }
        [Column("LastName")]
        public string? LastName { get; set; }
        [Column("RefreshToken")]
        public string? RefreshToken { get; set; }
        [Column("RefreshTokenValidity")]
        public DateTime RefreshTokenValidity { get; set; }
        public string? RoleName { get; set; }

    }
}
