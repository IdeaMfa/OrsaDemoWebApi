using Microsoft.AspNetCore.Mvc;
using OrsaDemoModels.Entity;
using System.Threading.Tasks;
using OrsaDemoWebApi.Models.Interface;
using System.Collections.Generic;

namespace OrsaDemoWebApi.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class GetLocationController : Controller
    {

        private readonly IGeographyService _geographyService;

        public GetLocationController(IGeographyService geographyService)
        {

            _geographyService = geographyService;

        }

        [HttpGet]
        [Route("GetLocation/{ParentId}")]
        public async Task<List<Geography>> GetLocation(int ParentId)
        {

            var result = await _geographyService.GetLocation(ParentId);

            return result;

        }

        [HttpGet]
        [Route("GetAllLocations")]
        public async Task<List<Geography>> GetAllLocations()
        {

            var result = await _geographyService.GetAllLocations();

            return result;

        }

    }
}
