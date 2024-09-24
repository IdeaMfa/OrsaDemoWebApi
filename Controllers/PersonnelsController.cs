using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrsaDemoWebApi.Models.Interface;
using System.Threading.Tasks;
using OrsaDemoModels.Entity;
using Microsoft.AspNetCore.Http;
using OrsaDemoModels.Entity.VmModel;

namespace OrsaDemoWebApi.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class PersonnelsController : Controller
    {

        private readonly IPersonnelsService _PersonnelsService;

        public PersonnelsController(IPersonnelsService personnelsService)
        {

            _PersonnelsService = personnelsService;

        }

        [Route("SavePersonnels")]
        [HttpPost]
        public async Task<Personnels> SavePersonnels(Personnels personnels)
        {
            var result = await _PersonnelsService.AddpersonnelService(personnels);
            
            return result;

        }

        [Route("SavePersonnelMedia")]
        [HttpPost]
        public async Task<List<MediaLibrary>> SavePersonnelMedia(List<MediaLibrary> Media)
        {
            
            var result = await _PersonnelsService.PersonnelSavePhoto(Media);

            return result;

        }

        [Route("GetAllInstitutions")]
        [HttpGet]
        public async Task<List<ParamInstitution>> GetAllInstitutions()
        {

            var result = await _PersonnelsService.GetAllInstitutions();

            return result;

        }

        [Route("SaveInstitutionData")]
        [HttpPost]
        public async Task<bool> SaveInstitutionData(List<vmSaveInstitution> InstitutionData)
        {

            var result = await _PersonnelsService.SaveInstitutionData(InstitutionData);

            return result;

        }

    }

}
