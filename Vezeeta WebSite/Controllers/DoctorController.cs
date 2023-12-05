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
        private readonly IBaseRepo<Doctor> docrepo;
        private readonly IBaseRepo<Patient> patrepo;
        private readonly IBaseRepo<Booking> bookrepo;
        private readonly IBaseRepo<Time> timservice;
        private readonly IBaseRepo<Appointment> appservice;

        public DoctorController(IBaseRepo<Doctor> docrepo,IBaseRepo<Patient> patrepo,IBaseRepo<Booking> bookrepo,IBaseRepo<Time> timservice,IBaseRepo<Appointment> appservice)
        {
            this.docrepo = docrepo;
            this.patrepo = patrepo;
            this.bookrepo = bookrepo;
            this.timservice = timservice;
            this.appservice = appservice;
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
                var res = await appservice.CreateAsync(appointt);
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
                var res = await timservice.CreateAsync(t);
                if (res)
                {
                    return Ok(res);
                }
                return BadRequest(res);
            }
            return BadRequest();
        }
        [HttpPut("UpdateAppointmentDetails")]
        public async Task<IActionResult> UpdateAppointmentDetails(int timeId,[FromQuery]AppointmentUpdate model)
        {
            
            var oldtime = await timservice.GetByidAsync(timeId);
            var appointt = await appservice.GetByidAsync(oldtime.AppointmentId);
            if (ModelState.IsValid)
            {
                oldtime.time = model.time != null ? model.time : oldtime.time;
                appointt.DayOfWeek = model.day != 0 ? model.day : appointt.DayOfWeek;
                var res = await timservice.Update(oldtime);
                var res2 = await appservice.Update(appointt);
                if (res!=null&&res2!=null)
                {
                    return Ok(true);
                }
                return BadRequest();

            }
            return BadRequest();
        }
        [HttpDelete("DeleteAppointmentTime")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await timservice.Delete(id));
        }
        [HttpGet("GetDoctorBookings")]
        public async Task<IActionResult> GetDoctorBookings(string Id,int PNum=1,int PSize=5)
        {
            var bookins = bookrepo.GetAll().Skip((PNum-1)*PSize).Take(PSize).ToList().Where(b => b.DoctorId == Id);
            if (bookins != null)
            {
                var lst = new List<DoctorRequestDTO>();
                foreach (var book in bookins)
                {
                    var patient = await patrepo.GetByidAsync(book.PatientId);


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
                    NumOfPages = (int)Math.Ceiling(((decimal)(bookrepo.GetAll().Count() / PSize))),
                    Data = lst

                });
            }
            return BadRequest();
        }
        [HttpPut("ConfirmCheckUP")]
        public async Task<IActionResult> ConfirmCheckUP(int bookingid)
        {
            var book= await bookrepo.GetByidAsync(bookingid);
            
            if (book!=null)
            {
                book.Status = RequestStatus.Completed;
                var res = await bookrepo.Update(book);
                if (res!=null)
                {
                    var patient = await patrepo.GetByidAsync(book.PatientId);
                    patient.numofCompletedRequest += 1;
                    if (patient.numofCompletedRequest==5)
                    {
                        RedirectToAction("CreateDisCountCode", "Admin", new {id=patient.Id});
                        
                    }
                    var res2 = await patrepo.Update(patient);
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
