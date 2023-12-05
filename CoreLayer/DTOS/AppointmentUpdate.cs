using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreLayer.DTOS
{
    public class AppointmentUpdate
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]

        public DayOfWeek day { set; get; }
        public string time { set; get; }
    }
}
