using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Models
{
    public enum RequestStatus
    {
        Pendeing, Completed, Canceled
    }
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(name: "Doctor")]
        public string DoctorId { set; get; }
        public virtual Doctor  Doctor { set; get; }
        public string Specialization { set; get; }
        [ForeignKey(name:"Time")]
        public int TimeId { set; get; }
        public virtual Time Time { get; set; }
        [ForeignKey(name: "DiscoundCode")]
        public int? DisCodeId { set; get; }
        
        public virtual DiscoundCode DiscoundCode { set; get; }
        public RequestStatus Status { set; get; }=RequestStatus.Pendeing;
        [ForeignKey(name: "Patient")]
        public string PatientId { set; get; }
        public virtual Patient Patient { set; get; }

    }
}
