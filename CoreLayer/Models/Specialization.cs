using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models
{
    public class Specialization
    {
        [Key]
        public int Id { set;get; }
        [Required]
        public string Name { set;get; }
        public virtual ICollection<Doctor> Doctors { set; get; }=new HashSet<Doctor>();
    }
}
