using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.DTOS
{
    public class TimeDTO
    {
        [Required]
        public string time { set; get; }
        [Required]
        public int AppointmentId { set; get; }
    }
}
