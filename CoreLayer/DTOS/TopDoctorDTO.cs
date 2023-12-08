using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.DTOS
{
    public class TopDoctorDTO
    {
        public string DocName { get; set; }
        public string Specialization { get; set; }

        public string Pic { get; set; }
        public int   NumofReq { get; set; }
    }
}
