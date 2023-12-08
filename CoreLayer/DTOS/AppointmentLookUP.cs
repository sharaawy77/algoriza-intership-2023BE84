using CoreLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreLayer.DTOS
{
    public class AppointmentLookUP
    {
        public int TimeId { set; get; }
        public string DocName { set; get; }
        public string DocSpecialization { set; get; }
        public string Email { set; get; }
        public string Mobile { set; get; }
        public string Pic { set; get; }
       

        public Gender? Gender { set; get; }
       

        public Tuple<DayOfWeek,string> Times {  set; get; }
        //public AppointmentLookUP(Doctor doctor)
        //{
        //    DocName = doctor.FirstName + " " + doctor.LastName;
        //    Email = doctor.Email;
        //    Mobile = doctor.PhoneNumber;
        //    Pic = doctor.Image;
        //    Gender = doctor.gender;

        //}


    }
}
