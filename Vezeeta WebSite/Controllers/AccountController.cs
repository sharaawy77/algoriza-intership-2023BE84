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
        private readonly IUnitOfWork unitOfWork;
        
        private readonly JWT jwtService;
        private readonly IFileService fileService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(IUnitOfWork unitOfWork,JWT jwtService,IFileService  fileService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.unitOfWork = unitOfWork;
           
           
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
            var AllTimes = unitOfWork.timrepo.GetAll();
            var AllBookings = unitOfWork.bookrepo.GetAll();
            var UsedTimesId = AllBookings.Select(b => b.TimeId);
            var AllAviablbeTimes = new List<Time>();
            foreach (var time in AllTimes)
            {
                if (UsedTimesId.All(timeId=>timeId!=time.Id))
                {
                    AllAviablbeTimes.Add(time);
                }
            }
            var AvailableTimes =  AllAviablbeTimes.Skip((PNum-1)*PSize).Take(PSize);
            foreach (var time in AvailableTimes)
            {
                var appoint =await  unitOfWork.apprepo.GetByidAsync(time.AppointmentId);
                var dayWeek = appoint.DayOfWeek;
                var doc = await unitOfWork.docrepo.GetByidAsync(appoint.DoctorId);
                var spec = await unitOfWork.specrepo.GetByidAsync(doc.SpecializationId);
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
        public async Task<IActionResult> Reserve(int timeId, string UserID)
        {
            
            if (timeId!=0)
            {
                var time = await unitOfWork.timrepo.GetByidAsync(timeId);
                var appoint = await unitOfWork.apprepo.GetByidAsync(time.AppointmentId);
                var doc = await unitOfWork.docrepo.GetByidAsync(appoint.DoctorId);
                var spec = await unitOfWork.specrepo.GetByidAsync(doc.SpecializationId);
                var patient = await unitOfWork.patrepo.GetByidAsync(UserID);
                int? disID = null;
                if (patient.numofCompletedRequest>=5)
                {
                    var code =  unitOfWork.disrepo.GetAll().FirstOrDefault(d => d.PatientId == null);
                    if (code!=null)
                    {
                        disID = code.Id ;
                        code.PatientId = UserID;
                        await unitOfWork.disrepo.Update(code);
                        patient.numofCompletedRequest = 0;
                        await unitOfWork.patrepo.Update(patient);
                    }
                    
                }
                //var handler = new JwtSecurityTokenHandler();
                //var jwttoken= handler.ReadJwtToken(token);
                //var userId = jwttoken.Claims.FirstOrDefault(claim => claim.Type == "NameIdentifier")?.Value;
                var Request = new Booking()
                {
                    TimeId = timeId,
                    DoctorId = doc.Id,
                    DisCodeId = disID,
                    Specialization = spec.Name,
                    PatientId =  UserID,
                    
                };
                if (Request!=null)
                {
                    var res = await unitOfWork.bookrepo.CreateAsync(Request);
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
            var bookins = unitOfWork.bookrepo.GetAll().ToList().Where(b => b.PatientId == Id);
            if (bookins!=null)
            {
                var lst = new List<BookingDTO>();
                foreach (var book in bookins)
                {
                    var patient = await unitOfWork.patrepo.GetByidAsync(Id);
                    var doc = await unitOfWork.docrepo.GetByidAsync(book.DoctorId);
                    var spec = await unitOfWork.specrepo.GetByidAsync(doc.SpecializationId);
                    var time = await unitOfWork.timrepo.GetByidAsync(book.TimeId);
                    var appoint = await unitOfWork.apprepo.GetByidAsync(time.AppointmentId);
                    var dis = await unitOfWork.disrepo.GetByidAsync(book.DisCodeId);
                    int? fPrice = null;
                    Guid? code = dis!=null?dis.code:null;
                    if (dis!=null)
                    {
                        if (dis.discountType==DiscountType.Value)
                        {
                            fPrice = appoint.Price - dis.Value;
                        }
                        else
                        {
                            fPrice = ((appoint.Price) - (int)((appoint.Price * dis.Value) / 100));
                        }
                    }
                    var b = new BookingDTO()
                    {
                        BookinId=book.Id,
                        DoctorName = doc.FirstName + " " + doc.LastName,
                        Pic = doc.Image,
                        Specialization = spec.Name,
                        Day = appoint.DayOfWeek,
                        Time = time.time,
                        PatientName = patient.UserName,
                        Price = appoint.Price,
                        Status = book.Status,
                        DisCode = code.HasValue ? code.Value : null,
                        finalPrice = fPrice != null ? fPrice : appoint.Price,
                    };
                    lst.Add(b);
                    
                }
                return Ok(lst);
            }
            return BadRequest();
        }


    }
}
