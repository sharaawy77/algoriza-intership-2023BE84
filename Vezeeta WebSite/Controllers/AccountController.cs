using CoreLayer.DTOS;
using CoreLayer.Interface;
using CoreLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using Vezeeta_WebSite.Utilities;

namespace Vezeeta_WebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IBaseRepo<DiscoundCode> disrepo;
        private readonly IBaseRepo<Patient> patrepo;
        private readonly IBaseRepo<Booking> bookrepo;
        private readonly IBaseRepo<Specialization> specrepo;
        private readonly IBaseRepo<Appointment> apprepo;
        private readonly IBaseRepo<Time> timrepo;
        private readonly IBaseRepo<Doctor> docservice;
        private readonly JWT jwtService;
        private readonly IFileService fileService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(IBaseRepo<DiscoundCode> disrepo,IBaseRepo<Patient> patrepo,IBaseRepo<Booking> bookrepo,IBaseRepo<Specialization> specrepo,IBaseRepo<Appointment> apprepo,IBaseRepo<Time> timrepo,IBaseRepo<Doctor> docservice,JWT jwtService,IFileService  fileService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.disrepo = disrepo;
            this.patrepo = patrepo;
            this.bookrepo = bookrepo;
            this.specrepo = specrepo;
            this.apprepo = apprepo;
            this.timrepo = timrepo;
            this.docservice = docservice;
            this.jwtService = jwtService;
            this.fileService = fileService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm]RegisterUser model)
        {
            if (ModelState.IsValid)
            {
                string ImagePath = null;
                if (model.Image!=null)
                {
                    var saving = fileService.SaveImage(model.Image);
                    if (saving.Item1==1)
                    {
                        ImagePath = saving.Item2;
                    }
                }
                var patient = new Patient()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber=model.MobileNumber,
                    FirstName=model.FirstName,
                    LastName=model.LastName,
                    DateOfBirth=model.DateOfBirth,
                    gender=model.gender,
                    Image=ImagePath,
                };
                var result = await userManager.CreateAsync(patient, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return BadRequest(ModelState);
                }
                else
                {
                    var res = await userManager.AddToRoleAsync(patient, "Patient");
                    if (res.Succeeded)
                    {
                        return Ok("Created");

                    }
                    return BadRequest("Error");
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser model)
        {
            if (ModelState.IsValid)
            {
                var user = new EmailAddressAttribute().IsValid(model.Email) ? userManager.FindByEmailAsync(model.Email).Result?.UserName??"" : model.Email;
                var patient = await userManager.FindByNameAsync(user);
                var result = await signInManager.PasswordSignInAsync(user, model.Password ,false,false);
                if (result.Succeeded)
                {
                    if (patient!=null)
                    {
                        var res = jwtService.CreateJWT(patient.Id, patient.UserName, userManager.GetRolesAsync(patient).Result.ToList());
                        return Ok(new
                        {
                            token=res.Item1,
                            expiration=res.Item2,
                            userId=patient.Id,
                        });
                    }
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return Unauthorized();
        }
        [HttpGet("GetAllAvailableAppointment")]
        public async Task<IActionResult> GetAllAvailableAppointment(int PNum=1,int PSize=5,string DocNAme=null)
        {
            var lst = new List<AppointmentLookUP>();
            var AllTimes = timrepo.GetAll();
            var AvailableTimes =  AllTimes.Skip((PNum-1)*PSize).Take(PSize);
            foreach (var time in AvailableTimes)
            {
                var appoint =await  apprepo.GetByidAsync(time.AppointmentId);
                var dayWeek = appoint.DayOfWeek;
                var doc = await docservice.GetByidAsync(appoint.DoctorId);
                var spec = await specrepo.GetByidAsync(doc.SpecializationId);
                var appointment = new AppointmentLookUP()
                {
                    TimeId = time.Id,
                    Times = new Tuple<DayOfWeek, string>(dayWeek, time.time),
                    DocName = doc.FirstName + " " + doc.LastName,
                    Email=doc.Email,
                    Gender=doc.gender,
                    Mobile=doc.PhoneNumber,
                    Pic=doc.Image,
                    DocSpecialization=spec.Name,
                };
                lst.Add(appointment);

            }
            if (DocNAme!=null)
            {
                lst = lst.Where(d => (d.DocName).ToUpper().StartsWith(DocNAme.ToUpper())).ToList();
            }
            else
            {
                lst = lst.ToList();
            }
            return Ok(new
            {
                Page = PNum,
                NumOfTimess = lst.Count(),
                NumOfPages=(int)Math.Ceiling(((decimal)(lst.Count()/PSize))),
                Data = lst

            });

        }
        [HttpPost("ReserveAppointment")]
        public async Task<IActionResult> Reserve(int timeId, string UserID, int? DiscountCodeId = null)
        {
            
            if (timeId!=0)
            {
                var time = await timrepo.GetByidAsync(timeId);
                var appoint = await apprepo.GetByidAsync(time.AppointmentId);
                var doc = await docservice.GetByidAsync(appoint.DoctorId);
                var spec = await specrepo.GetByidAsync(doc.SpecializationId);
                //var handler = new JwtSecurityTokenHandler();
                //var jwttoken= handler.ReadJwtToken(token);
                //var userId = jwttoken.Claims.FirstOrDefault(claim => claim.Type == "NameIdentifier")?.Value;
                var Request = new Booking()
                {
                    TimeId = timeId,
                    DoctorId = doc.Id,
                    DisCodeId = DiscountCodeId,
                    Specialization = spec.Name,
                    PatientId =  UserID
                };
                if (Request!=null)
                {
                    var res = await bookrepo.CreateAsync(Request);
                    if (res)
                    {
                        return Ok(res);
                    }
                    return BadRequest(res);
                }
                return BadRequest();
            }
            return BadRequest();
        }
        [HttpGet("GetPatientBookings")]
        public  async Task<IActionResult> GetPatientBookings(string Id)
        {
            var bookins = bookrepo.GetAll().ToList().Where(b => b.PatientId == Id);
            if (bookins!=null)
            {
                var lst = new List<BookingDTO>();
                foreach (var book in bookins)
                {
                    var patient = await patrepo.GetByidAsync(Id);
                    var doc = await docservice.GetByidAsync(book.DoctorId);
                    var spec = await specrepo.GetByidAsync(doc.SpecializationId);
                    var time = await timrepo.GetByidAsync(book.TimeId);
                    var appoint = await apprepo.GetByidAsync(time.AppointmentId);
                    var dis = await disrepo.GetByidAsync(book.DisCodeId);
                    Guid? code = dis!=null?dis.code:null;
                    var b = new BookingDTO()
                    {
                        DoctorName = doc.FirstName + " " + doc.LastName,
                        Pic = doc.Image,
                        Specialization = spec.Name,
                        Day = appoint.DayOfWeek,
                        Time = time.time,
                        PatientName=patient.UserName,
                        Price = appoint.Price,
                        Status=book.Status,
                        DisCode=code.HasValue ? code.Value : null,
                    };
                    lst.Add(b);
                    
                }
                return Ok(lst);
            }
            return BadRequest();
        }


    }
}
