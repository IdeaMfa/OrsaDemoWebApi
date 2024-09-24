using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using OrsaDemoModels.Entity;
using OrsaDemoWebApi.Models.Interface;
using OrsaDemoModels.Entity.VmModel;
using Microsoft.AspNetCore.Http;

namespace OrsaDemoWebApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UpdatePersonnelController : Controller
    {

        private readonly IUpdatePersonnelService _updatePersonnelService;

        public UpdatePersonnelController(IUpdatePersonnelService updatePersonnelService)
        {

            _updatePersonnelService = updatePersonnelService; 

        }

        [HttpGet]
        [Route("UpdatePersonnelGetData/{Id}")]
        public async Task<vmListPersonnel> UpdatePersonnelGetData(int Id)
        {

            var result = await _updatePersonnelService.UpdatePersonnelGetData(Id);

            return result;

        }

        [HttpPut]
        [Route("UpdatePersonnelPutData")]
        public async Task<Personnels> UpdatePersonnelPutData(Personnels personnels)
        {

            var result = await _updatePersonnelService.UpdatePersonnelPutData(personnels);

            return result;

        }

        [HttpPut]
        [Route("DeletePersonnelPhotos")]
        public async Task<bool> DeletePersonnelPhotots(List<int> Ids)
        {
            var result = await _updatePersonnelService.DeletePersonnelPhotos(Ids);

            return result;
        }

        [HttpPut]
        [Route("DeleteInstitutionMedia")]
        public async Task<bool> DeleteInstitutionMedia(List<int> Ids)
        {
            var result = await _updatePersonnelService.DeleteInstitutionMedia(Ids);

            return result;
        }

        [HttpPut]
        [Route("UpdateInstitutionDb")]
        public async Task<bool> UpdateInstitutionDb(List<vmSaveInstitution> InstitutionsData)
        {
            var result = await _updatePersonnelService.UpdateInstitutionDbService(InstitutionsData);

            return result;
        }

        [HttpPut]
        [Route("InstitutionDataDelete")]
        public async Task<bool> InstitutionDataDelete(int Id)
        {
            var result = await _updatePersonnelService.InstitutionDataDeleteService(Id);

            return result;
        }

    }
}
