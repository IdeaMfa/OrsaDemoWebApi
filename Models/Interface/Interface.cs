using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OrsaDemoModels.Entity;
using OrsaDemoModels.Entity.VmModel;

namespace OrsaDemoWebApi.Models.Interface

{
    public interface IPersonnelsService
    {

        Task<Personnels> AddpersonnelService(Personnels personnels);
        Task<List<MediaLibrary>> PersonnelSavePhoto(List<MediaLibrary> Media);
        Task<List<ParamInstitution>> GetAllInstitutions();
        Task<bool> SaveInstitutionData(List<vmSaveInstitution> InstitutionData);


    }

    public interface IGeographyService
    {

        Task<List<Geography>> GetLocation(int ParentId);
        Task<List<Geography>> GetAllLocations();

    }

    public interface IListPersonnelsService
    {
        //Task<List<Personnels>> GetAllPersonnelsService();
        Task<List<vmListPersonnel>> GetAllPersonnelsService();

    }

    public interface IUpdatePersonnelService
    {

        Task<vmListPersonnel> UpdatePersonnelGetData(int Id);
        Task<Personnels> UpdatePersonnelPutData(Personnels personnels);
        Task<bool> DeletePersonnelPhotos(List<int> Ids);
        Task<bool> DeleteInstitutionMedia(List<int> Ids);
        Task<bool> UpdateInstitutionDbService(List<vmSaveInstitution> InstitutionsData);
        Task<bool> InstitutionDataDeleteService(int Id);

    }
}