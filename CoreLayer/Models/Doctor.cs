using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models
{
    public class Doctor:ApplicationUser
    {
        [ForeignKey(name: "Specialization")]
        public int SpecializationId { set; get; }
             
        public virtual Specialization Specialization { set; get; }
        public ICollection<Booking> Bookings { set; get; } = new HashSet<Booking>();

        public ICollection<Appointment> Appointments { set; get; }=new HashSet<Appointment>();
        
    }
}
