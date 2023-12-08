using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreLayer.Models
{
    public enum DiscountType
    {
        Percentage,Value
    }
    public class DiscoundCode
    {
        [Key]
        public int Id { set; get; }
        public Guid code { set; get; } = Guid.NewGuid();
        public bool IsActive { set; get; }=true;
        [ForeignKey(name: "Patient")]
        public string PatientId { set; get; }
        public virtual Patient Patient { set; get; }
        public virtual Booking Booking { set; get; }
        //[JsonConverter(typeof(JsonStringEnumConverter))]

        public DiscountType discountType { set; get; } = DiscountType.Value;
        public int Value { set; get; } = 50;
    }
}
