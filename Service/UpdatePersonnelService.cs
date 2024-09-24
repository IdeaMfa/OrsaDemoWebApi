using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using OrsaDemoModels.Entity;
using OrsaDemoModels.Entity.VmModel;
using OrsaDemoWebApi.Models;
using OrsaDemoWebApi.Models.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OrsaDemoWebApi.Service
{
    public class UpdatePersonnelService : IUpdatePersonnelService
    {

        private readonly ApplicationDbContext _db;

        public UpdatePersonnelService(ApplicationDbContext db)
        {

            _db = db;

        }

        public async Task<vmListPersonnel> UpdatePersonnelGetData(int Id)
        {

            var GetPersonnelFromDb = await _db.Personnels.FirstOrDefaultAsync(p => p.Id == Id);
            var personnelMedia = await _db.PersonnelMedia.Where(a => a.PersonnelId == Id && a.IsActive == true && a.IsDeleted == false).ToListAsync();
            var mediaLibrary = await _db.MediaLibrary.ToListAsync();
            var locationDataFromDb = await _db.Geography.ToListAsync();
            var personnelInstitutionMediaRelationDataAll = await _db.PersonnelInstitutionMediaRelation.Where(r => r.IsActive == true && r.IsDeleted == false).ToListAsync();
            var paramInstitutions = await _db.ParamInstitution.ToListAsync();
            var Institutions = await _db.Institution.ToListAsync();


            var vmPersonnel = new vmListPersonnel();

            vmPersonnel.Id = GetPersonnelFromDb.Id;
            vmPersonnel.Name = GetPersonnelFromDb.Name;
            vmPersonnel.Surname = GetPersonnelFromDb.Surname;
            vmPersonnel.PhoneNumber = GetPersonnelFromDb.PhoneNumber;
            vmPersonnel.Email = GetPersonnelFromDb.Email;
            vmPersonnel.DateOfBirth = GetPersonnelFromDb.DateOfBirth;
            vmPersonnel.MediaLibrary = new List<MediaLibrary>();
            vmPersonnel.PersonnelInstitution = null;
            vmPersonnel.PersonnelInstitutions = new List<vmPersonnelInstitutionData>();

            // Get personnel institutions
            List<PersonnelInstitutionMediaRelation> Relations = new List<PersonnelInstitutionMediaRelation>();
            var NonOrderedPersonnelInstitutions = Institutions.Where(i => i.PersonnelId == Id && i.IsActive == true && i.IsDeleted == false).ToList();
            var PersonnelInstitutions = NonOrderedPersonnelInstitutions.OrderBy(i => i.InstitutionNumber);

            foreach (var personnelInstitution in PersonnelInstitutions)
            {
                vmPersonnelInstitutionData InstitutionData = new vmPersonnelInstitutionData();

                InstitutionData.InstitutionId = personnelInstitution.InstitutionId;
                InstitutionData.InstitutionNumber = personnelInstitution.InstitutionNumber;
                foreach (var paramInstitution in paramInstitutions)
                {
                    if (paramInstitution.Id == personnelInstitution.Id)
                    {
                        InstitutionData.InstitutionName = paramInstitution.InstitutionName;
                    }
                }
                InstitutionData.GraduationYear = personnelInstitution.GraduationYear;

                // Get media data
                InstitutionData.InstitutionMediaLibrary = new List<MediaLibrary>();
                int instNumber = InstitutionData.InstitutionNumber;
                foreach (var _relation in personnelInstitutionMediaRelationDataAll)
                {

                    if (_relation.PersonnelTableId == Id && _relation.InstitutionNumber == instNumber)
                    { 
                    
                        foreach (var media in mediaLibrary)
                        {
                            if (media.Id == _relation.MediaId)
                            {
                                MediaLibrary medialib = new MediaLibrary();

                                medialib.Id = media.Id;
                                medialib.MediaName = media.MediaName;
                                medialib.MediaUrl = media.MediaUrl;

                                InstitutionData.InstitutionMediaLibrary.Add(medialib);
                            }
                        }
                    }
                }

                vmPersonnel.PersonnelInstitutions.Add(InstitutionData);
            }
            
            int CityIdInt; 
            int? CityParentId = null;

            // Get location data belongs to the personnel
            if (GetPersonnelFromDb.City != null)
            {

                CityIdInt = Int32.Parse(GetPersonnelFromDb.City);
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
            if (personnelMedia != null)
            {

                foreach (var personnelmedia in personnelMedia)
                {

                    personnelMediaIds.Add(personnelmedia.MediaId);

                }

                foreach (var medialibrary in mediaLibrary)
                {

                    if (personnelMediaIds.Contains(medialibrary.Id))
                    {

                        var mediaLibraryDummy = new MediaLibrary();

                        mediaLibraryDummy.Id = medialibrary.Id;
                        mediaLibraryDummy.MediaName = medialibrary.MediaName;
                        mediaLibraryDummy.MediaUrl = medialibrary.MediaUrl;

                        vmPersonnel.MediaLibrary.Add(mediaLibraryDummy);

                    }
                    
                }

            }
            else
            {

                vmPersonnel.MediaLibrary.Add(null);

            }

            return vmPersonnel;

        }
   
        public async Task<Personnels> UpdatePersonnelPutData(Personnels personnels)
        {
            if (personnels == null)
            {
                return null;
            }

            // Retrieve the existing entity from the database
            var existingPersonel = await _db.Personnels
                                            .AsNoTracking() // Ensure it’s not being tracked
                                            .FirstOrDefaultAsync(p => p.Id == personnels.Id);

            if (existingPersonel == null)
            {
                // Handle the case where the entity does not exist
                throw new InvalidOperationException("Entity not found.");
            }

            // Check if password is changed if not set the variable with the existing password
            if (personnels.Password == null)
            {

                personnels.Password = existingPersonel.Password;

            }
            if (personnels.City == null)
            {
                personnels.City = existingPersonel.City;
            }
             
            // Update the entity
            _db.Personnels.Attach(personnels); // Attach the entity to the context
            _db.Entry(personnels).State = EntityState.Modified; // Mark it as modified

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency issues
                Console.WriteLine($"Concurrency error: {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                // Handle other database update issues
                Console.WriteLine($"Database update error: {ex.Message}");
                throw;
            }

            return personnels;
        }

        public async Task<bool> DeletePersonnelPhotos(List<int> Ids)
        {

            if (Ids.Count > 0)
            {

                int LengthOfIds = Ids.Count;

                List<int> MediaIdList = new List<int>();

                for (int i = 0; i < LengthOfIds; i++)
                {

                    if (i != LengthOfIds - 1)
                    {

                        MediaIdList.Add(Ids[i]);

                    }

                }

                int personnelId = Ids[LengthOfIds - 1];

                var personnelMedia = await _db.PersonnelMedia.Where(a => a.PersonnelId == personnelId && a.IsActive == true && a.IsDeleted == false).ToListAsync();
                foreach (var personnelMedium in personnelMedia)
                {

                    if (MediaIdList.Contains((int)personnelMedium.MediaId))
                    {

                        personnelMedium.IsActive = false;
                        personnelMedium.IsDeleted = true;
                        _db.PersonnelMedia.Update(personnelMedium);
                        await _db.SaveChangesAsync();

                    }


                }
                
                return true;
                    
            }
            else
            {

                return false;

            }

        }

        public async Task<bool> DeleteInstitutionMedia(List<int> Ids)
        {
            var personnelInstitutionMediaRelationDataAll = await _db.PersonnelInstitutionMediaRelation.Where(r => r.IsActive == true && r.IsDeleted == false).ToListAsync();

            foreach (var personnelInstitutionMediaRelationData in personnelInstitutionMediaRelationDataAll)
            {
                if (Ids.Contains(personnelInstitutionMediaRelationData.MediaId))
                {
                    personnelInstitutionMediaRelationData.IsActive = false;
                    personnelInstitutionMediaRelationData.IsDeleted = true;

                    _db.PersonnelInstitutionMediaRelation.Update(personnelInstitutionMediaRelationData);
                }
            }

            await _db.SaveChangesAsync();

            return true;

        }

        public async Task<bool> UpdateInstitutionDbService(List<vmSaveInstitution> InstitutionsData)
        {
            if (InstitutionsData != null)
            {
                int PersonnelId = InstitutionsData[0].Institution.PersonnelId;

                var PersonnelInstitutionsFromDb = await _db.Institution.Where(i => i.PersonnelId == PersonnelId && i.IsActive == true && i.IsDeleted == false).ToListAsync();
                var PersonnelInstitutionMediaRelationFromDb = await _db.PersonnelInstitutionMediaRelation.Where(r => r.PersonnelTableId == PersonnelId && r.IsActive == true && r.IsDeleted == false).ToListAsync();

                // Determine new institution number
                int[] NewInstitutionNumbers = new int[InstitutionsData.Count];
                for (int i = 0; i < InstitutionsData.Count; i++)
                {
                    NewInstitutionNumbers[i] = 1000 * (i + 1);
                }

                int InstitutionsDataCounter = 0;
                foreach (var InstitutionData in InstitutionsData)
                {

                    var InstitutionInfo = InstitutionData.Institution;

                    // Update or add institution non-media data
                    int FlagDoesNumberHave = 0;
                    foreach (var institution in PersonnelInstitutionsFromDb)
                    {   
                        if (InstitutionInfo.InstitutionNumber == institution.InstitutionNumber)
                        {
                            //InstitutionDummy.Id = institution.Id;
                            institution.InstitutionId = InstitutionInfo.InstitutionId;
                            institution.GraduationYear = InstitutionInfo.GraduationYear;
                            institution.PersonnelId = PersonnelId;
                            institution.IsActive = true;
                            institution.IsDeleted = false;
                            institution.InstitutionNumber = NewInstitutionNumbers[InstitutionsDataCounter] + 1;

                            _db.Institution.Update(institution);
                            continue;
                        }
                        FlagDoesNumberHave++;
                    }
                    Institution InstitutionDummy = new Institution();
                    if (FlagDoesNumberHave == PersonnelInstitutionsFromDb.Count)
                    {
                        InstitutionDummy.InstitutionId = InstitutionInfo.InstitutionId;
                        InstitutionDummy.GraduationYear = InstitutionInfo.GraduationYear;
                        InstitutionDummy.PersonnelId = PersonnelId;
                        InstitutionDummy.IsActive = true;
                        InstitutionDummy.IsDeleted = false;
                        InstitutionDummy.InstitutionNumber = NewInstitutionNumbers[InstitutionsDataCounter] + 1;

                        _db.Institution.Add(InstitutionDummy);
                    }
                      
                    var InstitutionMedia = InstitutionData.MediaLibrary;

                    // Update institution media data
                    // Check if the institution is newly added
                    if (InstitutionInfo.InstitutionNumber % 1000 != 0 || InstitutionInfo.InstitutionNumber > (1000 * PersonnelInstitutionsFromDb.Count))
                    {
                        foreach (var institutionMedia in InstitutionMedia)
                        {
                            PersonnelInstitutionMediaRelation relation = new PersonnelInstitutionMediaRelation();

                            _db.MediaLibrary.Add(institutionMedia);
                            await _db.SaveChangesAsync();

                            relation.PersonnelTableId = PersonnelId;
                            relation.MediaId = (int)institutionMedia.Id;
                            relation.IsActive = true;
                            relation.IsDeleted = false;
                            relation.InstitutionId = InstitutionInfo.InstitutionId;
                            relation.InstitutionNumber = NewInstitutionNumbers[InstitutionsDataCounter] + 1;

                            _db.PersonnelInstitutionMediaRelation.Add(relation);
                        }
                    }
                    else
                    {
                        var correspondingInstitutionMedia = PersonnelInstitutionMediaRelationFromDb.Where(r => r.InstitutionNumber == InstitutionInfo.InstitutionNumber).ToList();
                        foreach (var _correspondingInstitutionMedia in correspondingInstitutionMedia)
                        {
                            //relation.TableId = _correspondingInstitutionMedia.TableId;
                            //_correspondingInstitutionMedia.PersonnelTableId = PersonnelId;
                            //_correspondingInstitutionMedia.MediaId = _correspondingInstitutionMedia.MediaId;
                            //_correspondingInstitutionMedia.IsActive = _correspondingInstitutionMedia.IsActive;
                            //_correspondingInstitutionMedia.IsDeleted = _correspondingInstitutionMedia.IsDeleted;
                            _correspondingInstitutionMedia.InstitutionId = InstitutionInfo.InstitutionId;
                            _correspondingInstitutionMedia.InstitutionNumber = NewInstitutionNumbers[InstitutionsDataCounter] + 1;

                            _db.PersonnelInstitutionMediaRelation.Update(_correspondingInstitutionMedia);
                        }

                        PersonnelInstitutionMediaRelation newrelation = new PersonnelInstitutionMediaRelation();
                        foreach (var institutionMedia in InstitutionMedia)
                        {
                            _db.MediaLibrary.Add(institutionMedia);
                            await _db.SaveChangesAsync();

                            newrelation.PersonnelTableId = PersonnelId;
                            newrelation.MediaId = (int)institutionMedia.Id;
                            newrelation.IsActive = true;
                            newrelation.IsDeleted = false;
                            newrelation.InstitutionId = InstitutionInfo.InstitutionId;
                            newrelation.InstitutionNumber = NewInstitutionNumbers[InstitutionsDataCounter] + 1;

                            _db.PersonnelInstitutionMediaRelation.Add(newrelation);
                        }
                    }

                    InstitutionsDataCounter++;

                }

                await _db.SaveChangesAsync();

                // Rearange the institution numbers add and update opreations
                var InstitutionsAfterOperations = await _db.Institution.Where(i => i.PersonnelId == PersonnelId && i.IsActive == true && i.IsDeleted == false).ToListAsync();
                var InstitutionsMediaRelationsAfterOperations = await _db.PersonnelInstitutionMediaRelation.Where(r => r.PersonnelTableId == PersonnelId && r.IsActive == true && r.IsDeleted == false).ToListAsync();

                foreach (var personnelInstitutionsFromDb in InstitutionsAfterOperations)
                {
                    if (personnelInstitutionsFromDb.InstitutionNumber % 1000 != 0)
                    {
                        personnelInstitutionsFromDb.InstitutionNumber = personnelInstitutionsFromDb.InstitutionNumber - 1;
                        _db.Institution.Update(personnelInstitutionsFromDb);
                    }
                }

                foreach (var personnelInstitutionMediaRelationFromDb in InstitutionsMediaRelationsAfterOperations)
                {
                    if (personnelInstitutionMediaRelationFromDb.InstitutionNumber % 1000 != 0)
                    {
                        personnelInstitutionMediaRelationFromDb.InstitutionNumber = personnelInstitutionMediaRelationFromDb.InstitutionNumber - 1;
                        _db.PersonnelInstitutionMediaRelation.Update(personnelInstitutionMediaRelationFromDb);
                    }
                }

                await _db.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> InstitutionDataDeleteService(int Id)
        {

            if (Id != null)
            {
                var InstitutionsFromDb = await _db.Institution.Where(i => i.IsActive == true && i.IsDeleted == i.IsDeleted == false).ToListAsync();
                var personnelInstitutionMediaRelationDataAll = await _db.PersonnelInstitutionMediaRelation.Where(r => r.IsActive == true && r.IsDeleted == false).ToListAsync();

                foreach (var institution in InstitutionsFromDb)
                {
                    if (institution.InstitutionId == Id)
                    {
                        institution.IsDeleted = true;
                        institution.IsActive = false;

                        _db.Institution.Update(institution);
                    }
                }

                foreach (var relation in personnelInstitutionMediaRelationDataAll)
                {
                    if (relation.InstitutionId == Id)
                    {
                        relation.IsDeleted = true;
                        relation.IsActive = false;
                    }

                    _db.PersonnelInstitutionMediaRelation.Update(relation);
                }

                await _db.SaveChangesAsync();

                foreach ( var institution in InstitutionsFromDb)
                {

                }

                foreach (var relation in personnelInstitutionMediaRelationDataAll)
                {

                }

                return true;
            }
            else
            {
                return false;
            }

        }
    }

}
