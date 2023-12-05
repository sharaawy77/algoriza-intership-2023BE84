using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models
{
    //public enum Days
    //{
    //    Sat,Sun,Mon,Tus,Wedn,Tur,Fri
    //}
    public class Appointment
    {
        [Key]
        public int Id { set; get; }
        public DayOfWeek DayOfWeek { set; get; }
        [ForeignKey(name: "Doctor")]
        public string DoctorId { set; get; }
        public virtual Doctor Doctor { set; get; }
        public virtual ICollection<Time> Time { set; get; }=new HashSet<Time>();
        public int Price { set; get; }
    }
}
