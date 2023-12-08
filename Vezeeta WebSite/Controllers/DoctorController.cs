using CoreLayer.DTOS;
using CoreLayer.Interface;
using CoreLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Vezeeta_WebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
       

        public DoctorController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
           
        }
        [HttpPost("CreateAppointment")]
        public async Task<IActionResult> CreateAppointment([FromForm]AppointmentDTO appointment)
        {
            if (ModelState.IsValid)
            {
                var appointt = new Appointment()
                {
                    DoctorId=appointment.DoctorId,
                    Price=appointment.Price,
                    DayOfWeek=appointment.DayOfWeek,
                };
                var res = await unitOfWork.apprepo.CreateAsync(appointt);
                if (res)
                {
                    return Ok(res);
                }
                return BadRequest(res);
            }
            return BadRequest();
        }
        [HttpPost("CreateAppointmentTimes")]
        public async Task<IActionResult> CreateAppointmentTimes(TimeDTO time)
        {
            if (ModelState.IsValid)
            {
                var t = new Time()
                {
                    time = time.time,
                    AppointmentId = time.AppointmentId,
                    
                };
                var res = await unitOfWork.timrepo.CreateAsync(t);
                if (res)
                {
                    return Ok(res);
                }
                return BadRequest(res);
            }
            return BadRequest();
        }
        [HttpPut("UpdateTime")]
        public async Task<IActionResult> UpdateTime(int timeId,string time=null)
        {
            var bookings = unitOfWork.bookrepo.GetAll().ToList();
            if (bookings.All(b=>b.TimeId!=timeId))
            {
                var oldtime = await unitOfWork.timrepo.GetByidAsync(timeId);
                //var appointt = await unitOfWork.apprepo.GetByidAsync(oldtime.AppointmentId);
                if (ModelState.IsValid&&oldtime!=null)
                {
                    oldtime.time = time != null ? time : oldtime.time;
                    //appointt.DayOfWeek = model.day != 0 ? model.day : appointt.DayOfWeek;
                    var res = await unitOfWork.timrepo.Update(oldtime);
                    //var res2 = await unitOfWork.apprepo.Update(appointt);
                    if (res != null )
                    {
                        return Ok(true);
                    }
                    return BadRequest("Server Error");

                }
                return BadRequest("Error With TimeId");
            }
            
            return BadRequest("Appointment Already in Use");
        }
        [HttpPut("UpdateDay")]
        public async Task<IActionResult> UpdateDay(int appointId, DayOfWeek day)
        {
           
              
                var appointment = await unitOfWork.apprepo.GetByidAsync(appointId);
                if (appointment != null)
                {
                    
                    appointment.DayOfWeek = day ;
                    var res = await unitOfWork.apprepo.Update(appointment);
                    //var res2 = await unitOfWork.apprepo.Update(appointt);
                    if (res != null)
                    {
                        return Ok(true);
                    }
                    return BadRequest("Server Error");

                }
                return BadRequest("Error With AppointmentId");
        }

          
      
        [HttpDelete("DeleteAppointmentTime")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await unitOfWork.timrepo.Delete(id);
            if (res)
            {
                return Ok(res);
            }
            return BadRequest(false);
        }
        [HttpGet("GetDoctorBookings")]
        public async Task<IActionResult> GetDoctorBookings(string Id,int PNum=1,int PSize=5)
        {
            var bookins = unitOfWork.bookrepo.GetAll().Skip((PNum-1)*PSize).Take(PSize).ToList().Where(b => b.DoctorId == Id&&b.Status==RequestStatus.Pendeing);
            if (bookins != null)
            {
                var lst = new List<DoctorRequestDTO>();
                foreach (var book in bookins)
                {
                    var patient = await unitOfWork.patrepo.GetByidAsync(book.PatientId);


                    var b = new DoctorRequestDTO()
                    {
                        RequestId=book.Id,
                        PatientName = patient.FirstName + " " + patient.LastName,
                        Pic=patient.Image,
                        Email=patient.Email,
                        Mobile=patient.PhoneNumber,
                        Gender=patient.gender,
                        Age=(DateTime.Now.Year)-(patient.DateOfBirth.Value.Year)
                    };
                    lst.Add(b);

                }
                return Ok(new
                {
                    Page = PNum,
                    NumOfDocs = PSize,
                    NumOfPages = (int)Math.Ceiling(((decimal)(unitOfWork.bookrepo.GetAll().Count() / PSize))),
                    Data = lst

                });
            }
            return BadRequest();
        }
        [HttpPut("ConfirmCheckUP")]
        public async Task<IActionResult> ConfirmCheckUP(int bookingid)
        {
            var book= await unitOfWork.bookrepo.GetByidAsync(bookingid);
            
            if (book!=null)
            {
                book.Status = RequestStatus.Completed;
                var res = await unitOfWork.bookrepo.Update(book);
                if (res!=null)
                {
                    var patient = await unitOfWork.patrepo.GetByidAsync(book.PatientId);
                    patient.numofCompletedRequest += 1;
                    var res2 = await unitOfWork.patrepo.Update(patient);
                    
                    
                    if (res2!=null)
                    {
                        return Ok(true);
                    }
                    return BadRequest(false);

                }

                return BadRequest(false);
            }
            return BadRequest(false);
        }
    }
}
