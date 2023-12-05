using CoreLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreLayer.DTOS
{
    public class BookingDTO
    {
        public string DoctorName { set; get; }
        public string Pic { set; get; }
        public string Specialization { set; get; }
        [JsonConverter(typeof(JsonStringEnumConverter))]

        public DayOfWeek Day { set; get; }
        public  string Time { get; set; }
        
        public Guid? DisCode { set; get; }

       
        public RequestStatus Status { set; get; } = RequestStatus.Pendeing;
        
        public string PatientName { set; get; }
        public  int Price { set; get; }
        //public int finalPrice { set; get; }
    }
}
