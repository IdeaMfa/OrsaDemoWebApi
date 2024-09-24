using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrsaDemoModels.Entity;
using OrsaDemoWebApi.Models;
using OrsaDemoWebApi.Models.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrsaDemoWebApi.Service
{

    public class GetLocationService : IGeographyService
    {

        private readonly ApplicationDbContext _db;

        public GetLocationService(ApplicationDbContext db)
        {

            _db = db;

        }

        public async Task<List<Geography>> GetLocation(int ParentId)
        {

            var Locations = await _db.Geography.Where(g => g.ParentId == ParentId).ToListAsync();
            return Locations;

        }

        public async Task<List<Geography>> GetAllLocations()
        {

            var LocationsList = await _db.Geography.ToListAsync();

            if (LocationsList == null)
            {
                return null;
            }

            return LocationsList;
            
        }

    }

}
