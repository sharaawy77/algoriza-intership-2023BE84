using CoreLayer.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Interface
{
    public interface IRankingService
    {
        public List<Tuple<string,int>> GetTopSpecilizations(int num);
        public Task< List<TopDoctorDTO>> GetTopDoctors(int num);

    }
}
