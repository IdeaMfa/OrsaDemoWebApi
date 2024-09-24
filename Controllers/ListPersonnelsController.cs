using Microsoft.AspNetCore.Mvc;
using OrsaDemoModels.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using OrsaDemoWebApi.Models.Interface;
using OrsaDemoModels.Entity.VmModel;

namespace OrsaDemoWebApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ListPersonnelsController : Controller
    {

        private readonly IListPersonnelsService _listpersonnelsservice;

        public ListPersonnelsController(IListPersonnelsService listpersonnelsservice)
        {

            _listpersonnelsservice = listpersonnelsservice;

        }

        [Route("ListPersonnels")]
        [HttpGet]
        public async Task<List<vmListPersonnel>> ListPersonnels()
        {

            var result = await _listpersonnelsservice.GetAllPersonnelsService();
            
            return result;

        }

    }

}
