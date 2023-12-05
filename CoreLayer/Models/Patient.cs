using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models
{
    public class Patient:ApplicationUser
    {
        [ForeignKey(name: "BookingTime")]
        public int? TimeId { set; get; }
        public virtual Time BookingTime { set; get; }

        public int numofCompletedRequest { set; get; } = 0;
        public virtual ICollection<Booking> Bookings { set; get; } = new HashSet<Booking>();
        public virtual ICollection<DiscoundCode> DiscoundCodes { set; get; } = new HashSet<DiscoundCode>();

    }
}
