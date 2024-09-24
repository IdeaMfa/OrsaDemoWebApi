using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using OrsaDemoWebApi.Models.Interface;

namespace OrsaDemoWebApi.Service.IoC
{
    public static class ServiceContainer
    {

        public static void AddScopedService(this IServiceCollection services)
        {

            services.AddScoped<IPersonnelsService, AddPersonnelService>();
            services.AddScoped<IGeographyService, GetLocationService>();
            services.AddScoped<IListPersonnelsService, ListPersonnelsService>();
            services.AddScoped<IUpdatePersonnelService, UpdatePersonnelService>();

        }

    }
}
