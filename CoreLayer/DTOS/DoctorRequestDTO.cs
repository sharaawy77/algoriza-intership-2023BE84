using CoreLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreLayer.DTOS
{
    public class DoctorRequestDTO
    {
        public int RequestId { set;get; }
        public string PatientName { set; get; }
        public string Pic { set; get;}
        public string Mobile { set; get; }
        public string Email { set; get; }
        public int Age { set; get; }
       

        public Gender? Gender { set; get; }

    }
}
