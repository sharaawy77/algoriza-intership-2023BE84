using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models
{
    public enum Gender
    {
        Female, Male
    }
    public abstract class ApplicationUser:IdentityUser
    {
        [AllowNull]
        public string FirstName { get; set; }
        [AllowNull]

        public string LastName { get; set; }
        public string Image { get; set; }
        [AllowNull]

        public DateTime? DateOfBirth { get; set; }
        [AllowNull]

        public Gender? gender { get; set; }
        



    }
}
