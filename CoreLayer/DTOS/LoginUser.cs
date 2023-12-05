using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CoreLayer.DTOS
{
    public class LoginUser
    {
        [Required]
        [Display(Name = "UserName OR Email")]
        public string Email { get; set; }

        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
