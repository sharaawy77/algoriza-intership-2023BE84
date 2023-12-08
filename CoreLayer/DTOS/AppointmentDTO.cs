using CoreLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreLayer.DTOS
{
    public class AppointmentDTO
    {
        [Required]
        
        public DayOfWeek DayOfWeek { set; get; }
        [Required]
        public string DoctorId { set; get; }
        [Required]
        public int Price { set; get; }
       
    }
}
