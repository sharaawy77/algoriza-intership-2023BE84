using CoreLayer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CoreLayer.DTOS
{
    public class DoctorDTO
    {
        
        public string FullName { get; set; }
       
        public string Email { get; set; }


        
        public string MobileNumber { get; set; }
       
        public string Image { get; set; }
       

        public DateTime? DateOfBirth { get; set; }
    
        public string Specialization { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]

        public Gender? gender { get; set; }
        
        public DoctorDTO(Doctor model)
        {
            FullName = model.FirstName + model.LastName;
            Email = model.Email;
            MobileNumber = model.PhoneNumber;
            Image = model.Image;
            DateOfBirth = model.DateOfBirth;
            gender = model.gender;
            //Specialization = model.Specialization.Name;
            
        }
    }
}
