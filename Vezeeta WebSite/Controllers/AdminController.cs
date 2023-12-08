using CoreLayer.DTOS;
using CoreLayer.Interface;
using CoreLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicesLayer.Services;
using Vezeeta_WebSite.Utilities;

namespace Vezeeta_WebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IRankingService ranking;
        private readonly IUnitOfWork unitOfWork;
       
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFileService fileService;

        public AdminController(IRankingService ranking,IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager,IFileService fileService)
        {
            this.ranking = ranking;
            this.unitOfWork = unitOfWork;
           
            this.userManager = userManager;
            this.fileService = fileService;
        }
        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctor(int SpecilizatinId,[FromForm]RegisterDoctor model)
        {
            if (ModelState.IsValid)
            {
                string ImagePath = null;
                
                var saving = fileService.SaveImage(model.Image);
                if (saving.Item1 == 1)
                {
                   ImagePath = saving.Item2;
                }
               
                var doctor = new Doctor()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.MobileNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    gender = model.gender,
                    Image = ImagePath,
                    SpecializationId=SpecilizatinId
                };
                var result = await userManager.CreateAsync(doctor, model.Password);
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
                    var res = await userManager.AddToRoleAsync(doctor, "Doctor");
                    if (res.Succeeded)
                    {
                        return Ok("Created");

                    }
                    return BadRequest("Error");
                }
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetDoctorById")]
        public async Task<IActionResult> Details(string id)
        {
            var doc = await unitOfWork.docrepo.GetByidAsync(id);
            var Spec = await unitOfWork.specrepo.GetByidAsync(doc.SpecializationId);
            var DocDto = new DoctorDTO(doc);
            DocDto.Specialization = Spec.Name;
            if (doc!=null)
            {
                return Ok(DocDto);
            }
            return BadRequest("Cant Find Doctor ");
        }
        [HttpGet("GetAllDoctors")]
        public async Task<IActionResult> GetAll(int pageSize=5,int PageNum=1)
        {
            
            var Docs =  unitOfWork.docrepo.GetAll();
            var ReqDocs = await Docs.Skip((PageNum - 1) * pageSize).Take(pageSize).ToListAsync();
            var DocsVM = new List<DoctorDTO>();
            foreach (var ReqDoc in ReqDocs)
            {
                var DocVM=new DoctorDTO(ReqDoc);
                DocVM.Specialization =  unitOfWork.specrepo.GetByidAsync(ReqDoc.SpecializationId).Result.Name;
                DocsVM.Add(DocVM);
            }
            if (DocsVM!=null)
            {
                return Ok(new
                {
                    Page=PageNum,
                    NumOfDocs=pageSize,
                    NumOfPages = (int)Math.Ceiling(((decimal)(DocsVM.Count() / pageSize))),
                    Data =DocsVM

                });
            }
            return BadRequest();
        }
        [HttpPut("UpdateDoctorInfo")]
        public async Task<IActionResult> Update(string id,[FromForm]UpdatedDoctor model)
        {
            if (ModelState.IsValid)
            {
                var doc = await unitOfWork.docrepo.GetByidAsync(id);
                var oldpath = doc.Image;

                string newpath = null;
                if (model.Image!=null)
                {

                    var result =fileService.DeleteImage(oldpath);
                    if (result)
                    {
                        var saving = fileService.SaveImage(model.Image);
                        if (saving.Item1 == 1)
                        {
                            newpath = saving.Item2;
                        }
                    }
                }
                if (doc != null)
                {
                    doc.FirstName = model.FirstName != null ? model.FirstName : doc.FirstName;
                    doc.LastName = model.LastName != null ? model.LastName : doc.LastName;
                    doc.Image = newpath!=null?newpath:oldpath;
                    doc.gender = model.gender != null ? model.gender : doc.gender;
                    doc.Email = model.Email != null ? model.Email : doc.Email;
                    doc.DateOfBirth = model.DateOfBirth != null ? model.DateOfBirth : doc.DateOfBirth;
                    doc.PhoneNumber = model.MobileNumber != null ? model.MobileNumber : doc.PhoneNumber;
                    
                    var newdoc = await unitOfWork.docrepo.Update(doc);
                    if (newdoc!=null)
                    {
                        return Ok(new DoctorDTO(newdoc));

                    }
                    return BadRequest("Server Error");


                }
                return BadRequest();
            }
            return BadRequest();
        }
        [HttpGet("GetNumsOfDoctors")]
        public IActionResult GetNumsOfDoctors()
        {
            return Ok( unitOfWork.docrepo.GetAll().Count());
        }
        [HttpGet("GetNumsOfPatients")]
        public  IActionResult GetNumsOfPatient()
        {
            return Ok(unitOfWork.patrepo.GetAll().Count());
        }
        [HttpDelete("RemoveDoctor")]
        public async Task<IActionResult> RemoveDoctor(string id)
        {
            var Bookings = unitOfWork.bookrepo.GetAll().ToList();
            if (Bookings.All(b=>b.DoctorId!=id))
            {
                var res = await unitOfWork.docrepo.Delete(id);
                if (res)
                {
                    return Ok(res);

                }
                return BadRequest(res);
            }
            return BadRequest("Doctor Already Have Requests");
            
        }
        [HttpPost("CreateDisCountCode")]
        public async Task<IActionResult> CreateDisCountCode(DicCountCodeDTO model)
        {
            var DisCode = new DicCountCodeDTO();
            if (ModelState.IsValid)
            {
                DisCode.discountType = model.discountType;
                DisCode.Value =model.Value ;
            }
            
            var Code = new DiscoundCode()
            {
                discountType=DisCode.discountType,
                Value=DisCode.Value,
            };
            var res = await unitOfWork.disrepo.CreateAsync(Code);
            if (res)
            {
                return Ok(res);

            }
            return BadRequest(false);


        }
        [HttpPut("UpdateDiscountCodeDetails")]
        public async Task<IActionResult> UpdateDiscountCodeDetails(int id, DicCountCodeDTO model)
        {
            var code = await unitOfWork.disrepo.GetByidAsync(id);
            if (code!=null&&code.PatientId==null)
            {
                code.discountType = model.discountType;
                code.Value = model.Value;
                var res = await unitOfWork.disrepo.Update(code);
                if (res!=null)
                {
                    return Ok(true);
                }
                return BadRequest(false);

            }
           
            return BadRequest("Code Is Already In Use OR Error In Id");
        }
        [HttpDelete("DeleteDiscountCode")]
        public async Task<IActionResult> DeleteDiscountCode(int id)
        {
            var code = await unitOfWork.disrepo.GetByidAsync(id);
            if (code != null && code.PatientId == null)
            {
                
                var res = await unitOfWork.disrepo.Delete(id);
                if (res )
                {
                    return Ok(true);
                }
                return BadRequest(false);

            }

            return BadRequest("Code Is Already In Use OR Error In Id");
        }
        [HttpGet("GetTopSpecialization")]
        public IActionResult GetTopSpecialization(int num)
        {
            var res = ranking.GetTopSpecilizations(num);
            if (res!=null)
            {
                return Ok(res);
            }
            return BadRequest();
        }
        [HttpGet("GetTopDoctors")]
        public async Task<IActionResult> GetTopDoctors(int num)
        {
            var res =await ranking.GetTopDoctors(num);
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest();
        }
    }

}
