using System;
using System.Threading.Tasks;
using OrsaDemoModels.Entity;
using OrsaDemoWebApi.Models.Interface;
using OrsaDemoWebApi.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;
using OrsaDemoModels.Entity.VmModel;
//using OrsaDemoWebApi.Helpers;

namespace OrsaDemoWebApi.Service
{
    public class AddPersonnelService : IPersonnelsService
    {

        private readonly ApplicationDbContext _db;
        public AddPersonnelService(ApplicationDbContext db)
        {

            _db = db;

        }

        public async Task<Personnels> AddpersonnelService(Personnels personnels)
        {

            if (personnels != null)
            {

                 _db.Personnels.Add(personnels);
                await _db.SaveChangesAsync();
                
                return personnels;

            }
            else
            {

                return null;

            }

        }

        public async Task<List<ParamInstitution>> GetAllInstitutions()
        {

            var institutionsList = await _db.ParamInstitution.ToListAsync();

            return institutionsList;

        }

        public async Task<List<MediaLibrary>> PersonnelSavePhoto(List<MediaLibrary> Media)
        {

            var IdContainer = new PersonnelMedia();
            var MediaLibraryId = new List<long>();
            foreach (var photo in Media)
            {

                IdContainer.PersonnelId = (int)photo.Id;
                photo.Id = 0;
                _db.MediaLibrary.Add(photo);
                await _db.SaveChangesAsync();
                MediaLibraryId.Add(photo.Id);
                        
            }
            foreach (var personnelMedia in MediaLibraryId)
            {

                var personnelMediaSave = new PersonnelMedia()
                {

                    PersonnelId = IdContainer.PersonnelId,
                    MediaId = (int)personnelMedia,
                    IsActive = true,
                    IsDeleted = false,

                };

                _db.Add(personnelMediaSave);
                await _db.SaveChangesAsync();

            }

            return Media;

        }

        public async Task<bool> SaveInstitutionData(List<vmSaveInstitution> InstitutionData)
        {

            List<vmSaveInstitution> InstitutionDataToDb = new List<vmSaveInstitution>();

            var Institutions = await _db.ParamInstitution.ToListAsync();
            List<int> InstitutionIds = new List<int>();
            
            foreach (var institution in Institutions)
            {

                InstitutionIds.Add(institution.Id);

            }

            foreach (var institutionData in InstitutionData)
            {

                if (InstitutionIds.Contains(institutionData.Institution.InstitutionId))
                {

                    InstitutionDataToDb.Add(institutionData);

                }

            }

            if (InstitutionDataToDb.Count != InstitutionData.Count)
            {

                return false; 

            }

            
            
            foreach (var institution in InstitutionDataToDb)
            {

                _db.Institution.Add(institution.Institution);

                int counter = 0;
                List<MediaLibrary> MediaLibrariesToRelation = new List<MediaLibrary>();
                foreach (var mediaLibrary in institution.MediaLibrary)
                {

                    // Add corresponding media object into MediaLibrary table
                    _db.MediaLibrary.Add(mediaLibrary);

                    MediaLibrariesToRelation.Add(mediaLibrary);

                    counter++;

                }

                // Save changes db
                await _db.SaveChangesAsync();

                for (int i = 0; i < counter; i++)
                {

                    // Add corresponding media relation object into PersonnelInstitutionMediaRelation table by doing needed actions
                    PersonnelInstitutionMediaRelation personnelInstitutionMediaRelation = new PersonnelInstitutionMediaRelation();
                    personnelInstitutionMediaRelation.PersonnelTableId = institution.Institution.PersonnelId;
                    personnelInstitutionMediaRelation.MediaId = (int)MediaLibrariesToRelation[i].Id;
                    personnelInstitutionMediaRelation.IsActive = true;
                    personnelInstitutionMediaRelation.IsDeleted = false;
                    personnelInstitutionMediaRelation.InstitutionId = institution.Institution.InstitutionId;
                    personnelInstitutionMediaRelation.InstitutionNumber = institution.Institution.InstitutionNumber;
                    
                    _db.PersonnelInstitutionMediaRelation.Add(personnelInstitutionMediaRelation);

                }

                // Save changes db
                await _db.SaveChangesAsync();

            }

            return true;

        }

    }

}
