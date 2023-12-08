using CoreLayer.DTOS;
using CoreLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Services
{
    public class RankingService:IRankingService
    {
        private readonly IUnitOfWork unitOfWork;

        public RankingService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<List<TopDoctorDTO>> GetTopDoctors(int num)
        {
            var TopDocs=new List<TopDoctorDTO>();
            var Booking = unitOfWork.bookrepo.GetAll();
            var res=Booking.GroupBy(B=>B.DoctorId).ToList();
            var topDocs=res.OrderByDescending(res=>res.Count());
            var TopReqDocs = topDocs.Take(num);
            foreach (var item in topDocs)
            {
                var doc = await unitOfWork.docrepo.GetByidAsync(item.Key);
                if (doc!=null)
                {
                    var topdoc = new TopDoctorDTO()
                    {
                        DocName = doc.FirstName + " " + doc.LastName,
                        Pic = doc.Image,
                        Specialization =  unitOfWork.specrepo.GetByidAsync(doc.SpecializationId).Result.Name,
                        NumofReq=item.Count(),
                    };
                    TopDocs.Add(topdoc);
                }
            }
            return TopDocs;
        }

        public  List<Tuple<string,int>> GetTopSpecilizations(int num)
        {

            var TopSpecialization=new List<Tuple<string,int>>();
            var bookings =  unitOfWork.bookrepo.GetAll();

            var res=bookings.GroupBy((B)=>B.Specialization).ToList();
            var topSpec=res.OrderByDescending((res)=>res.Count());
            var TopReqSpecs=topSpec.Take(num);
            foreach( var item in TopReqSpecs)
            {
                TopSpecialization.Add(new Tuple<string,int>(item.Key,item.Count()));
            }
            return TopSpecialization;


        }
    }
}
