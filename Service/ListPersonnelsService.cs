using Microsoft.EntityFrameworkCore;
using OrsaDemoModels.Entity;
using OrsaDemoModels.Entity.VmModel;
using OrsaDemoWebApi.Models;
using OrsaDemoWebApi.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrsaDemoWebApi.Service
{
    public class ListPersonnelsService : IListPersonnelsService
    {

        private readonly ApplicationDbContext _db;

        public ListPersonnelsService(ApplicationDbContext db)
        {

            _db = db;

        }

        public async Task<List<vmListPersonnel>> GetAllPersonnelsService()
        {
            
            var personnelsList = await _db.Personnels.ToListAsync();
            var personnelMedia = await _db.PersonnelMedia.Where(a => a.IsActive == true && a.IsDeleted == false).ToListAsync();
            var mediaLibrary = await _db.MediaLibrary.ToListAsync();
            var locationDataFromDb = await _db.Geography.ToListAsync();
            var personnelInstitutionMediaRelationDataAll = await _db.PersonnelInstitutionMediaRelation.Where(r => r.IsActive == true && r.IsDeleted == false).ToListAsync();
            var institutions = await _db.ParamInstitution.ToListAsync();

            var vmPersonnels = new List<vmListPersonnel>();

            int CityIdInt;
            int? CityParentId = null;
            foreach (var personnel in personnelsList)
            {

                var vmPersonnel = new vmListPersonnel();

                vmPersonnel.Id = personnel.Id;
                vmPersonnel.Name = personnel.Name;
                vmPersonnel.Surname = personnel.Surname;
                vmPersonnel.PhoneNumber = personnel.PhoneNumber;
                vmPersonnel.Email = personnel.Email;
                vmPersonnel.DateOfBirth = personnel.DateOfBirth;
                vmPersonnel.MediaLibrary = new List<MediaLibrary>();
                vmPersonnel.PersonnelInstitution = new vmPersonnelInstitution();

                List<PersonnelInstitutionMediaRelation> personnelInstitutionMediaRelationData = new List<PersonnelInstitutionMediaRelation>();
                // Get Personnel Institution data
                foreach (var _personnelInstitutionMediaRelationDataAll in personnelInstitutionMediaRelationDataAll)
                {

                    if (_personnelInstitutionMediaRelationDataAll.PersonnelTableId == personnel.Id)
                    {

                        personnelInstitutionMediaRelationData.Add(_personnelInstitutionMediaRelationDataAll);

                    }

                }

                if (personnelInstitutionMediaRelationData != null)
                {

                    // Get last institution media
                    PersonnelInstitutionMediaRelation lastInstitutionMedia = new PersonnelInstitutionMediaRelation();
                    if (personnelInstitutionMediaRelationData.Count > 0)
                    {

                        lastInstitutionMedia = personnelInstitutionMediaRelationData[personnelInstitutionMediaRelationData.Count - 1];

                    }

                    // Get last instition Id
                    int lastInstitutionId = lastInstitutionMedia.InstitutionId;
                    vmPersonnel.PersonnelInstitution.LastInstitutionId = lastInstitutionId;

                    // Get last institution media id
                    int lastInstitutionMediaId = lastInstitutionMedia.MediaId;

                    // Get last institution MediaLibrary object
                    MediaLibrary lastMedia = new MediaLibrary();
                    foreach (var _mediaLibrary in mediaLibrary)
                    {
                        
                        if (_mediaLibrary.Id == lastInstitutionMediaId)
                        {
                            lastMedia = _mediaLibrary;
                        }

                    }

                    // Get last institution media url
                    var lastInstitutionMediaUrl = lastMedia.MediaUrl;
                    vmPersonnel.PersonnelInstitution.LastInstitutionUrl = lastInstitutionMediaUrl;

                    foreach (var institution in institutions)
                    {

                        if (institution.Id == lastInstitutionId)
                        {

                            vmPersonnel.PersonnelInstitution.LastInstitutionName = institution.InstitutionName;

                        }

                    }

                }

                // Get location data belongs to the personnel
                if (personnel.City != null)
                {

                    CityIdInt = Int32.Parse(personnel.City);
                    foreach (var location in locationDataFromDb)
                    {

                        if (CityIdInt == location.Id)
                        {
                            vmPersonnel.City = location.RegionName;
                            CityParentId = location.ParentId;
                        }

                    }

                    foreach (var location in locationDataFromDb)
                    {
                        if (CityParentId == location.Id)
                        {
                            vmPersonnel.Country = location.RegionName;
                        }
                    }

                }
                else
                {

                    vmPersonnel.City = null;
                    vmPersonnel.Country = null;

                }

                // Get media data belongs to the personnel
                var personnelMediaIds = new List<long>();
                foreach (var personnelmedia in personnelMedia)
                {

                    if (personnelmedia.PersonnelId == personnel.Id)
                    {

                        personnelMediaIds.Add(personnelmedia.MediaId);
                        
                    }

                }
                
                if (personnelMediaIds.Count > 0)
                {

                    foreach (var medialibrary in mediaLibrary)
                    {

                        if (personnelMediaIds.Contains(medialibrary.Id))
                        {

                            var personnelMediaDummy = new MediaLibrary();

                            personnelMediaDummy.Id = medialibrary.Id;
                            personnelMediaDummy.MediaName = medialibrary.MediaName;
                            personnelMediaDummy.MediaUrl = medialibrary.MediaUrl;

                            vmPersonnel.MediaLibrary.Add(personnelMediaDummy);

                        }

                    }
                }
                else
                {

                    vmPersonnel.MediaLibrary.Add(null);

                }


                
                vmPersonnels.Add(vmPersonnel);

            }

            return vmPersonnels;

        }

    }

}
