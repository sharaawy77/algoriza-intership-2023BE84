using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models
{
   
    public class Time
    {
        [Key]
        public int Id { set; get; }
        public string  time {  set; get; }
        [ForeignKey(name: "Appointment")]
        public int AppointmentId { set; get; }
        public virtual Appointment Appointment { set; get; }
        public virtual Booking Booking { set; get; }
       
    }
}
