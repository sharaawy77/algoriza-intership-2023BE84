using CoreLayer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreLayer.DTOS
{
    public class UpdatedDoctor
    {
        //public string Id { set; get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        public string Email { get; set; }



        public string MobileNumber { get; set; }

        public IFormFile Image { get; set; }


        public DateTime? DateOfBirth { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]

        public Gender? gender { get; set; }
    }
}
