using CoreLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreLayer.DTOS
{
    public class DicCountCodeDTO
    {
        [Required]
        public Guid code { set; get; } = new Guid();
        [Required]
        public bool IsActive { set; get; } = true;

        [JsonConverter(typeof(JsonStringEnumConverter))]

        public DiscountType discountType { set; get; } = DiscountType.Value;
        [Required]
        public int Value { set; get; } = 50;
    }
}
