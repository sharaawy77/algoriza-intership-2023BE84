using CoreLayer.DTOS;
using CoreLayer.Interface;
using CoreLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vezeeta_WebSite.Utilities;

namespace Vezeeta_WebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IBaseRepo<DiscoundCode> disrepo;
        private readonly IBaseRepo<Patient> patrepo;
        private readonly IBaseRepo<Specialization> specrepo;
        private readonly IBaseRepo<Doctor> docrepo;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFileService fileService;

        public AdminController(IBaseRepo<DiscoundCode> disrepo,IBaseRepo<Patient> patrepo,IBaseRepo<Specialization> Specrepo,IBaseRepo<Doctor> docrepo,UserManager<ApplicationUser> userManager,IFileService fileService)
        {
            this.disrepo = disrepo;
            this.patrepo = patrepo;
            specrepo = Specrepo;
            this.docrepo = docrepo;
            this.userManager = userManager;
            this.fileService = fileService;
        }
        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctor([FromQuery]int SpecilizatinId,[FromForm]RegisterDoctor model)
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
            var doc = await docrepo.GetByidAsync(id);
            var Spec = await specrepo.GetByidAsync(doc.SpecializationId);
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
            
            var Docs =  docrepo.GetAll();
            var ReqDocs = await Docs.Skip((PageNum - 1) * pageSize).Take(pageSize).ToListAsync();
            var DocsVM = new List<DoctorDTO>();
            foreach (var ReqDoc in ReqDocs)
            {
                var DocVM=new DoctorDTO(ReqDoc);
                DocVM.Specialization =  specrepo.GetByidAsync(ReqDoc.SpecializationId).Result.Name;
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
                var doc = await docrepo.GetByidAsync(id);
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
                    
                    var newdoc = await docrepo.Update(doc);

                    return Ok(new DoctorDTO(newdoc));


                }

            }
            return BadRequest();
        }
        [HttpGet("GetNumsOfDoctors")]
        public IActionResult GetNumsOfDoctors()
        {
            return Ok( docrepo.GetAll().Count());
        }
        [HttpGet("GetNumsOfPatients")]
        public  IActionResult GetNumsOfPatient()
        {
            return Ok(patrepo.GetAll().Count());
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveDoctor(string id)
        {
            return Ok(await docrepo.Delete(id));
        }
        [HttpPost("CreateDisCountCode")]
        public async Task<IActionResult> CreateDisCountCode(string id)
        {

            var disCode = new DiscoundCode();
            disCode.PatientId = id;
            var res = await disrepo.CreateAsync(disCode);
            if (res)
            {
                return Ok(res);
            }
            return BadRequest(false);
        } 
    }
}
